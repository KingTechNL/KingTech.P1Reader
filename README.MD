# P1 Reader

This is a simple reader for the P1 port of Dutch smart meters. 
It is based on the [P1 protocol](https://www.netbeheernederland.nl/_upload/Files/Slimme_meter_15_a727fce1f1.pdf).

The P1 port is a serial port that is used to communicate with the smart meter. Communication is possible using a [P1 to usb](https://www.robbshop.nl/slimme-meter-kabel-usb-p1-1-meter) 
serial cable. Using the KingTech P1 reader we can read incomming traffic from this serial connection.

Received data is made available via (REST) API and Prometheus metrics.

## Docker image
The KingTech P1 Reader is published as a Docker image.
The Docker image can be used to run the P1 reader in a container.

## NuGet
The KingTech P1 Reader is published as [NuGet package](https://www.nuget.org/packages/KingTech.P1Reader.NuGet/).
The NuGet can be used by any C# application to receive events from the P1 port. 

## Example P1 telegram message
Below follows an example message captured from the P1 port of the smart meter at my home:

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