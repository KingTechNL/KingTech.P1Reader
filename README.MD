# P1 Reader

This is a simple reader for the P1 port of Dutch smart meters. 
It is based on the [P1 protocol](https://www.netbeheernederland.nl/_upload/Files/Slimme_meter_15_a727fce1f1.pdf).

The P1 port is a serial port that is used to communicate with the smart meter. Communication is possible using a [P1 to usb](https://www.robbshop.nl/slimme-meter-kabel-usb-p1-1-meter) 
serial cable. Using the KingTech P1 reader we can read incomming traffic from this serial connection.

Received data is made available via (REST) API and Prometheus metrics.
It is also possible to automatically push all data to [DSMR reader](https://github.com/dsmrreader/dsmr-reader).

# Usage
The KingTech P1 Reader is published as a [docker image](https://hub.docker.com/repository/docker/kingtechnl/kingtech_p1reader/general), but can also be compiled to run stand-alone. 
In addition to the web-application, a [NuGet package](https://www.nuget.org/packages/KingTech.P1Reader.NuGet/) is published for developers to handle data received from P1 ports.

## Docker image
The KingTech P1 Reader is published as a [docker image](https://hub.docker.com/repository/docker/kingtechnl/kingtech_p1reader/general).
The Docker image can be used to run the P1 reader in a container.

In order to run the docker container, the serial device needs to be mounted.
The application can be configured by setting the following environment variables:

The serial port can be specified, but also needs to corrospond to the mounted device.
``` 
P1ReceiverSettings__SerialPort=/dev/ttyUSB0 
```
The format of the telegram messages can be specified, by default it is XS210ESMR5.
```
P1ReceiverSettings__Format=XS210ESMR5
```

### Compose example

```
kingtech_p1reader
  container_name: kingtech_p1reader
  image: kingtechnl/kingtech_p1reader:latest
  restart: always
  devices:
    - /dev/ttyUSB0:/dev/ttyUSB0
  ports:
    - 8888:80
  environment:
    - P1ReceiverSettings__SerialPort=/dev/ttyUSB0
    - P1ReceiverSettings__Format=XS210ESMR5
```

## Stand-alone
The KingTech P1 Reader is also compiled and released as a stand-alone application.
The application can be configured using the standard (asp).NET appsettings.json file.
An example of this configuration file is added to this repository.

### Installing as linux service
The application can be installed as a linux (ubuntu/debian) service. For this purpose, the `kingtech-p1reader.service` unit file is created. 
In order to install the KingTech P1Reader, copy this file to the `/etc/systemd/system` directory and update the paths to the executable and working directory.

Next, let us reload `systemctl` to make our new unit file visible.
```
sudo systemctl daemon-reload
```
Now enable our service using the command below:
```
sudo systemctl enable kingtech-p1reader.service
```
Finally, we start the service using the following command:
```
sudo systemctl start kingtech-p1reader.service
```

To make sure the service is running, the following command can be used:
```
systemctl status kingtech-p1reader
```

Optionally, the following command can be used to list all systemctl services:
``` 
systemctl list-units --type=service 
```

## NuGet
The KingTech P1 Reader is published as [NuGet package](https://www.nuget.org/packages/KingTech.P1Reader.NuGet/).
The NuGet can be used by any C# application to receive events from the P1 port. 

To receive messages, a `P1Receiver` object needs to be created. When using dependency injection, it is recommended to register this class as singleton as it actively listens on the specified serial port.
The `P1Receiver` object requires a `P1ReceiverSettings` object to configure the serial port and message format.

To open the connection with the serial port and start listening for incomming messages, call the `Start()` method on the `P1Receiver`. To close the connection again the `Stop()` method.

# Development
This repository is split into 2 projects. The [KingTech.P1Receiver.NuGet](#nuget) project which handles the connection with the P1 port, and the KingTech.P1Receiver project that exposes this data.

To support more P1 formats, new implementations of the `P1Parser` can be added to the KingTech.P1Receiver.NuGet project. By adding it to the case switch in the `GetParser()` method in the `P1Receiver` class.

Once a connection to the serial port is made, the P1 connection will start sending [telegram](#example-p1-telegram-message) messages. These messages will be parsed and the resulting model is exposed as a C# event.

The KingTech.P1Reader project uses the NuGet to receive these telegram messages and exposes them on web-endpoints. A [REST endpoint](#rest-json-response) is made available to fetch data as json object, and a [metrics endpoint](#prometheus-metrics) exposes this same data in a format Prometheus can use.

## Example P1 telegram message
The energie meter sends telegram messages containing [detailed information](https://www.netbeheernederland.nl/_upload/Files/Slimme_meter_15_a727fce1f1.pdf) about received and delivered power.
Below follows an example message captured from the P1 port of the smart meter at my home:

```
1-3:0.2.8(50)
0-0:1.0.0(230205165005W)
0-0:96.1.1(4530303435303033383039333631343137)
1-0:1.8.1(011080.001*kWh)
1-0:1.8.2(011931.124*kWh)
1-0:2.8.1(000914.942*kWh)
1-0:2.8.2(001837.962*kWh)
0-0:96.14.0(0001)
1-0:1.7.0(00.404*kW)
1-0:2.7.0(00.000*kW)
0-0:96.7.21(00007)
0-0:96.7.9(00009)
1-0:99.97.0(9)(0-0:96.7.19)(190315191821W)(0000005100*s)(190521113136S)(0000000516*s)(211014073208S)(0000000615*s)(211022160832S)(0000005232*s)(211222113427W)(0000001441*s)(220331141855S)(0000008493*s)(220915122840S)(0000001629*s)(220915132057S)(0000002966*s)(221009193502S)(0000027164*s)
1-0:32.32.0(00046)
1-0:32.36.0(00014)
0-0:96.13.0()
1-0:32.7.0(229.0*V)
1-0:31.7.0(003*A)
1-0:21.7.0(00.404*kW)
1-0:22.7.0(00.000*kW)
0-2:24.1.0(003)
0-2:96.1.0(4730303332353635353433363436313137)
0-2:24.2.1(230205164505W)(06919.956*m3)
!8FF3
```

## Prometheus metrics
The values received via the serial connection are exposed via a [metrics endpoint](https://prometheus.io/docs/instrumenting/writing_exporters/) in the Prometheus [data format](https://prometheus.io/docs/concepts/data_model/).
This endpoint can be reached by navigating to `[ip:port]/metrics`. Below follows an example of the metrics data exposed by this service.

```
# HELP p1_actual_electricity_delivered Electricity actually delivered
# TYPE p1_actual_electricity_delivered gauge
p1_actual_electricity_delivered 1.335
# HELP process_start_time_seconds Start time of the process since unix epoch in seconds.
# TYPE process_start_time_seconds gauge
process_start_time_seconds 1675712932.0448544
# HELP process_private_memory_bytes Process private memory size
# TYPE process_private_memory_bytes gauge
process_private_memory_bytes 107245568
# HELP p1_power_failures_long Power failures long
# TYPE p1_power_failures_long gauge
p1_power_failures_long 9
# HELP p1_actual_electricity_retreived Electricity actually retreived
# TYPE p1_actual_electricity_retreived gauge
p1_actual_electricity_retreived 0
# HELP prometheus_net_metric_timeseries Number of metric timeseries currently generated from all metric instances.
# TYPE prometheus_net_metric_timeseries gauge
prometheus_net_metric_timeseries{metric_type="gauge"} 30
prometheus_net_metric_timeseries{metric_type="summary"} 0
prometheus_net_metric_timeseries{metric_type="histogram"} 0
prometheus_net_metric_timeseries{metric_type="counter"} 4
# HELP p1_active_tariff Active tariff
# TYPE p1_active_tariff gauge
p1_active_tariff 2
# HELP p1_usage_electricity_high Electricity usage high tariff
# TYPE p1_usage_electricity_high gauge
p1_usage_electricity_high 11941.425
# HELP process_num_threads Total number of threads
# TYPE process_num_threads gauge
process_num_threads 78
# HELP prometheus_net_meteradapter_instruments_connected_total Number of instruments that are currently connected to the adapter.
# TYPE prometheus_net_meteradapter_instruments_connected_total gauge
prometheus_net_meteradapter_instruments_connected_total 0
# HELP prometheus_net_eventcounteradapter_sources_connected_total Number of event sources that are currently connected to the adapter.
# TYPE prometheus_net_eventcounteradapter_sources_connected_total gauge
prometheus_net_eventcounteradapter_sources_connected_total 17
# HELP process_virtual_memory_bytes Virtual memory size in bytes.
# TYPE process_virtual_memory_bytes gauge
process_virtual_memory_bytes 2229689102336
# HELP p1_power_failures_short Power failures short
# TYPE p1_power_failures_short gauge
p1_power_failures_short 7
# HELP prometheus_net_metric_instances Number of metric instances currently registered across all metric families.
# TYPE prometheus_net_metric_instances gauge
prometheus_net_metric_instances{metric_type="gauge"} 30
prometheus_net_metric_instances{metric_type="summary"} 0
prometheus_net_metric_instances{metric_type="histogram"} 0
prometheus_net_metric_instances{metric_type="counter"} 4
# HELP prometheus_net_metric_families Number of metric families currently registered.
# TYPE prometheus_net_metric_families gauge
prometheus_net_metric_families{metric_type="gauge"} 22
prometheus_net_metric_families{metric_type="summary"} 0
prometheus_net_metric_families{metric_type="histogram"} 0
prometheus_net_metric_families{metric_type="counter"} 2
# HELP p1_returned_electricity_high Electricity returned high tariff
# TYPE p1_returned_electricity_high gauge
p1_returned_electricity_high 1838.909
# HELP dotnet_total_memory_bytes Total known allocated memory
# TYPE dotnet_total_memory_bytes gauge
dotnet_total_memory_bytes 10961456
# HELP process_working_set_bytes Process working set
# TYPE process_working_set_bytes gauge
process_working_set_bytes 81293312
# HELP dotnet_collection_count_total GC collection count
# TYPE dotnet_collection_count_total counter
dotnet_collection_count_total{generation="1"} 0
dotnet_collection_count_total{generation="2"} 0
dotnet_collection_count_total{generation="0"} 0
# HELP p1_usage_electricity_low Electricity usage low tariff
# TYPE p1_usage_electricity_low gauge
p1_usage_electricity_low 11092.658
# HELP process_open_handles Number of open handles
# TYPE process_open_handles gauge
process_open_handles 808
# HELP process_cpu_seconds_total Total user and system CPU time spent in seconds.
# TYPE process_cpu_seconds_total counter
process_cpu_seconds_total 1.125
# HELP p1_usage_gas Gas usage
# TYPE p1_usage_gas gauge
p1_usage_gas 6929.3193
# HELP p1_returned_electricity_low Electricity returned low tariff
# TYPE p1_returned_electricity_low gauge
p1_returned_electricity_low 914.942
```


## REST json response
In addition to the metrics endpoint, the data is also exposed in json format. For this, simply navigate to `[ip:port]/` . An example json output can be found below: 

```
{
  "timestamp": "2023-02-06T20:50:26",
  "rawTimestamp": 230206205026,
  "electricityUsageLow": 11092.658,
  "electricityUsageHigh": 11941.418,
  "electricityReturnedLow": 914.942,
  "electricityReturnedHigh": 1838.909,
  "activeTariff": 2,
  "powerFailuresShort": 7,
  "powerFailuresLong": 9,
  "actualElectricityDelivered": 1.274,
  "actualElectricityRetreived": 0,
  "gasUsage": 6929.3193
}
```



# P1 Format
The full [P1 format](https://www.netbeheernederland.nl/_upload/Files/Slimme_meter_15_a727fce1f1.pdf) specification (V5.0.2) consists of the fields listed below. Depending on your device some fields may or may not be included.

| Value | OBIS reference | Attribute | Class ID | Value Format | Value Unit |
| --- | --- | --- | --- | --- | --- |
| Header information | - | - | - | Manufacturer specific | |
| Version information for P1 output | 1-3:0.2.8.255 | 2 | 1 Data | S2, tag 9 | |
| Date-time stamp of the P1 message | 0-0:1.0.0.255 | 2 | 8 | TST | YYMMDDhhmmssX |
| Equipment identifier | 0-0:96.1.1.255 | 2 Value | 1 Data | Sn (n=0..96), tag 9 | |
| Meter Reading electricity delivered to client (Tariff 1) in 0,001 kWh | 1-0:1.8.1.255 | 2 Value | 3 Register | F9(3,3), tag 6 | kWh |
| Meter Reading electricity delivered to client (Tariff 2) in 0,001 kWh | 1-0:1.8.2.255 | 2 Value | 3 Register | F9(3,3), tag 6 | kWh |
| Meter Reading electricity delivered by client (Tariff 1) in 0,001 kWh |1-0:2.8.1.255 | 2 Value | 3 Register | F9(3,3), tag 6 | kWh |
| Meter Reading electricity delivered by client (Tariff 2) in 0,001 kWh |1-0:2.8.2.255 | 2 Value | 3 Register | F9(3,3), tag 6 | kWh |
| Tariff indicator electricity. The tariff indicator can also be used to switch tariff dependent loads e.g boilers. This is the responsibility of the P1 user | 0-0:96.14.0.255 | 2 Value | 1 Data | S4, tag 9 | |
| Actual electricity power delivered (+P) in 1 Watt resolution | 1-0:1.7.0.255 | 2 Value | 3 Register | F5(3,3), tag 18 | kW |
| Actual electricity power received (-P) in 1 Watt resolution | 1-0:2.7.0.255 | 2 Value | 3 Register | F5(3,3), tag 18 | kW |
| Number of power failures in any phase | 0-0:96.7.21.255 | 2 Value | 1 Data | F5(0,0), tag 18 | |
| Number of long power failures in any phase | 0-0:96.7.9.255 | 2 Value | 1 Data | F5(0,0), tag 18 | |
| Power Failure Event Log (long power failures) | 1-0:99.97.0.255 | 2 Buffer | 7 Profile Generic | TST, F10(0,0) - tag 6 Format applicable for the value within the log (OBIS code 0-0:96.7.19.255) | Timestamp (end of failure) – duration in seconds |
| Number of voltage sags in phase L1 | 1-0:32.32.0.255 | 2 Value | 1 Data | F5(0,0), tag 18 |
| Number of voltage sags in phase L2 | 1-0:52.32.0.255 | 2 Value | 1 Data | F5(0,0), tag 18 |
| Number of voltage sags in phase L3 | 1-0:72.32.0.255 | 2 Value | 1 Data | F5(0,0), tag 18 |
| Number of voltage swells in phase L1 | 1-0:32.36.0.255 | 2 Value | 1 Data | F5(0,0),  tag 18 |
| Number of voltage swells in phase L2 | 1-0:52.36.0.255 | 2 Value | 1 Data | F5(0,0),  tag 18 |
| Number of voltage swells in phase L3 | 1-0:72.36.0.255 | 2 Value | 1 Data | F5(0,0),  tag 18 |
| Text message max 1024 characters. | 0-0:96.13.0.255 | 2 Value | 1 Data | Sn (n=0..2048), tag 9 |
| Instantaneous voltage L1 in V resolution | 1-0:32.7.0.255 | 2 Value | 3 Register | F4(1,1), tag 18 | V |
| Instantaneous voltage L2 in V resolution | 1-0:52.7.0.255 | 2 Value | 3 Register | F4(1,1), tag 18 | V |
| Instantaneous voltage L3 in V resolution | 1-0:72.7.0.255 | 2 Value | 3 Register | F4(1,1), tag 18 | V |
| Instantaneous current L1 in A resolution. | 1-0:31.7.0.255 | 2 Value | 3 Register | F3(0,0), tag 18 | A |
| Instantaneous current L2 in A resolution. | 1-0:51.7.0.255 | 2 Value | 3 Register | F3(0,0), tag 18 | A |
| Instantaneous current L3 in A resolution. | 1-0:71.7.0.255 | 2 Value | 3 Register | F3(0,0), tag 18 | A |
| Instantaneous active power L1 (+P) in W resolution | 1-0:21.7.0.255 | 2 Value | 3 Register | F5(3,3), tag 18 | kW |
| Instantaneous active power L2 (+P) in W resolution | 1-0:41.7.0.255 | 2 Value | 3 Register | F5(3,3), tag 18 | kW |
| Instantaneous active power L3 (+P) in W resolution | 1-0:61.7.0.255 | 2 Value | 3 Register | F5(3,3), tag 18 | kW |
| Instantaneous active power L1 (-P) in W resolution | 1-0:22.7.0.255 | 2 Value | 3 Register | F5(3,3), tag 18 | kW |
| Instantaneous active power L2 (-P) in W resolution | 1-0:42.7.0.255 | 2 Value | 3 Register | F5(3,3), tag 18 | kW |
| Instantaneous active power L3 (-P) in W resolution | 1-0:62.7.0.255 | 2 Value | 3 Register | F5(3,3), tag 18 | kW |
| Device-Type | 0-n:24.1.0.255  | 9 Device | type 72 M-Bus client | F3(0,0), tag 17 |
| Equipment identifier (Gas) | 0-n:96.1.0.255 | 2 Value | 1 Data | Sn (n=0..96), tag 9 |
| Last 5-minute value (temperature converted), gas delivered to client in m3, including decimal values and capture time | 0-n:24.2.1.255 | 5 Capture time | 4 Extended Register | TST |
| Last 5-minute value (temperature converted), gas delivered to client in m3, including decimal values and capture time | 0-n:24.2.1.255 | 2 Value | 4 Extended Register | F8(2,2)/F8(3,3), tag 18 (See note 2) | m3 |
| Device-Type | 0-n:24.1.0.255 | 9 Device type | 72 M-Bus client F3(0,0), tag 17 |
| Equipment identifier (Thermal: Heat or Cold) | 0-n:96.1.0.255 | 2 Value | 1 Data | Sn (n=0..96), tag 9 |
| Last 5-minute Meter reading Heat or Cold in 0,01 GJ and capture time | 0-n:24.2.1.255 | 5 Capture time | 4 Extended Register | TST |
| Last 5-minute Meter reading Heat or Cold in 0,01 GJ and capture time | 0-n:24.2.1.255 | 2 Value | 4 Extended Register | Fn(2,2) (See note 1) | GJ |
| Device-Type | 0-n:24.1.0.255 | 9 Device type | 72 M-Bus client | F3(0,0), tag 17 |
| Equipment identifier (Water) | 0-n:96.1.0.255 | 2 Value | 1 Data | Sn (n=0..96), tag 9 |
| Last 5-minute Meter reading in 0,001 m3 and capture time | 0-n:24.2.1.255 | 5 Capture time | 4 Extended Register | TST |
| Last 5-minute Meter reading in 0,001 m3 and capture time | 0-n:24.2.1.255 | 2 Value | 4 Extended Register | Fn(3,3) (See Note 1) | m3 |
| Device-Type | 0-n:24.1.0.255 | 9 Device type | 72 M-Bus client | F3(0,0), tag 17 |
| Equipment identifier | 0-n:96.1.0.255 | 2 Value | 1 Data | Sn (n=0..96), tag 9 |
| Last 5-minute Meter reading and capture time (e.g. slave E meter) | 0-n:24.2.1.255 | 5 Capture time | 4 Extended Register | TST |
| Last 5-minute Meter reading and capture time (e.g. slave E meter) | 0-n:24.2.1.255 | 2 Value | 4 Extended Register | Fn(3,3) (See Note 1) | kWh |

Note 1: Where n is the number of digits before the decimal point on the display of the meter + the required number of decimals. So for example if on the physical display of a Heat meter is shown 1234 GJ (without decimals) the n=6 and the format of the P1 output will be 1234,56 GJ.

Note 2: For Gas meters with a capacity up to 10 m3 /h (G4 and G6) F8(3,3) is  applicable. For Gas meters with higher capacities F8(2,2) is applicable.