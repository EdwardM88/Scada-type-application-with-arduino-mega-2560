# Scada type application

Hi, for this project, I developed a system in C# that simulates the operation of a SCADA system, such as one found in a factory.As data-collection devices, we used real sensors—specifically, the ones that came in an Arduino kit.
The operation is fairly straightforward: the sensors collect data from the environment, transmit it to the microcontroller and the app, and based on various conditions, different devices are activated.

## 🛠️ Tech Stack & Technologies

* **Frontend / HMI:** C# .NET, WPF (Windows Presentation Foundation), XAML, DataGrid Styling.
* **Database:** MongoDB (C# Official Driver, BSON mapping, Async/Await operations).
* **Hardware / Embedded:** Arduino Mega 2560 (C++ / Wiring framework).
* **Communication:** System.IO.Ports (SerialPort), Custom Binary/String Framing Protocol.

## 🏗️ System Architecture


[ Industrial Sensors / Actuators ]
               │
               ▼
   [ Arduino Mega 2560 ]
               │  (Serial / UART Protocol)
               ▼
   [ SCADA Lite WPF Application ] ──► [ Local MongoDB Database ]

## Features

  * Data measured by the sensors can be saved in MongoDB.
  * You can see, in a graph, how the readings from the various sensors change over time.
  * Sensors and equipment can be turned on and off both manually and via the app.
  * Actuators have different speeds and operations based on sensor readings.
  * You can connect to the Arduino directly from the app.
  * If any disconnections or abnormal readings occur, an alarm system is triggered.

## 📋 System Requirements & Dependencies

To build, run, and test this project, ensure your environment meets the following hardware and software requirements:

### 🔌 Hardware Requirements
| Component | Specification / Details |
| :--- | :--- |
| **Microcontroller** | **Arduino Mega 2560** (ATmega2560 board) |
| **Connectivity** | USB Type-A to Type-B Cable (Serial/UART communication) |
| **Sensors & Actuators** | Analog/Digital industrial sensors & relay modules as Photoresistor, DC and Stepper Motors, DHT11 sensor, LEDs |

---

### 💻 Software & Framework Requirements
* **Operating System:** Windows 10 / 11 (required for WPF Desktop runtime)
* **Development Framework:** **.NET 6.0 SDK** (or higher) / .NET Framework 4.8+
* **Database Management:** **MongoDB Community Server** (v5.0+ running on `mongodb://localhost:27017`)
* **Database Tool:** MongoDB Compass *(optional, for visual data inspection)*

### 🛠️ Development Tools (IDEs & Libraries)

#### 1. Desktop Application (C# / WPF)
* **IDE:** [Visual Studio 2022](https://visualstudio.microsoft.com/) (with *.NET Desktop Development* workload installed)
* **NuGet Packages:**
  * `MongoDB.Driver` *(Official C# Driver for MongoDB persistence)*
  * `System.IO.Ports` *(Serial communication handling)*

#### 2. Embedded Firmware (C++)
* **IDE:** [Arduino IDE 2.x](https://www.arduino.cc/en/software) or [PlatformIO](https://platformio.org/) in VS Code
* **Board Core:** Arduino AVR Boards Package (`Arduino Mega or Mega 2560`)

## Configuration for MongoDB

  * Clone the project: Download or clone this repository to your computer.
  * Set the environment variables: Locate the file named `.env.example`, make a copy of it, and rename the copy to `.env`
  * Create the database: Sign up for a free account on [MongoDB Atlas](https://www.mongodb.com/cloud/atlas) (or use a local MongoDB server) and create a cluster.
  * Add your link: Copy your MongoDB connection link, open the `.env` file you just created, and paste the link into the `MONGO_URI` variable. Don’t forget to replace `<username>` and `<password>` with your actual database credentials!
  * Launch the app: You can now run the program, and any data you enter will be securely saved in your database.

## Circuit diagram
 <img width="500" height="500" alt="circuitDiagram" src="https://github.com/user-attachments/assets/63a92ed1-2ed9-4f37-bb86-03b6d8284d9e" />

  * S circle is my servo motor
  * Because DHT11 doesn't has a symbol,I reprezented him as a rectangle,and included ground,VDD and out wires.
