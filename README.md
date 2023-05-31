# Onyx USB-I2C/SPI Converter

>Onyx is a I2C bus and SPI bus host adapter which allows to communicate with any I2C or SPI devices via USB. DEICO Onyx is powered directly from the PC’s USB port. DEICO Onyx can be used as master or slave for I2C and SPI. I2C and SPI pins can also be used as GPIO or combined with I2C or SPI functionality. 

## Features
The general features of DEICO Onyx are listed below:
*	Plug and play with DEICO serial center
*	Device is powered from USB
*	Master or slave mode
*	Asynchronous slave transmit and receive
*	I2C interface supports various speeds from 100kHz to 3.4MHz
*	I2C interface supports multi-master
*	I2C interface internal pull-up resistors
*	SPI interface supports various speeds from 200kHz to 25MHz
*	SPI interface full duplex transmit and receive
*	SPI interface software configurable slave select polarity
*	I2C and SPI pins can be used as GPIO or combined with I2C or SPI

Areas of application include: 

*	EEPROMS
*	Flash 
*	Temperature Sensors
*	Accelerometers

## Electrical Specifications

| Specifications          | Minimum | Typical | Maximum | Notes |
| :---------------------- | :-----: | :------:| :-----: | :---------------- |
| Supply voltage          | 4.5V    | 5V      | 5.5V    |   -               |
| Supply current          | -       | -       |  0.4A   |   -               |
| Output voltage high     | 2.4V    | -       |  -      |   -               |
| Output voltage low      | -       | -       |  0.4V   |   -               |
| Input voltage high      | 2V      | -       |  -      |   -               |
| Input voltage low       | -       | -       |  0.8V   |   -               |
| Output current          | -       | -       |  20mA   |   Per signal pin  |
| VCC 3V3 Output Voltage  | 3.27V   | 3.3V    |  3.33V  |   -               |
| VCC 3V3 Output Current  | -       | -       |  100mA  |   DC              |

## Target Bus Connector

![Target Bus Connector] (/Images/Connector.png)

| Pin | Pin Name       | Description                                                     |
| :-: | :------------: | :-------------------------------------------------------------- |
| 1   | I2C SCL        | I2C interface clock signal. There is internal 4.7KOHM pull-up resistor.   This pin can be used as GPIO.|
| 2   | GND            | Ground connection |
| 3   | I2C SDA        | I2C interface data signal. There is internal 4.7KOHM pull-up resistor. This pin can be used as GPIO.   |
| 4   | VCC 3V3        | 3.3V output       |
| 5   | SPI MISO       | SPI interface master in slave out signal. This pin can be used as GPIO. |
| 6   | VCC 3V3        | 3.3V output       |
| 7   | SPI CLK        | SPI interface clock signal. This pin can be used as GPIO. |
| 8   | SPI MOSI       | SPI interface master out slave in signal. This pin can be used as GPIO. |
| 9   | SPI SS         | SPI interface slave select signal. This pin can be used as GPIO. |
| 10  | GND            | Ground connection |


## API
This repository contains samples for Onyx USB-I2C/SPI Converter in .NET platform.
