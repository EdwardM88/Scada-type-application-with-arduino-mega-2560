#include "DHT.h"
#include <Stepper.h>
#include <Servo.h>

//define DHT11 and DHT11 pin port
#define DHTPIN 2
#define DHTTYPE DHT11
DHT dht(DHTPIN, DHTTYPE);

int photoResistor = A0;

bool DHT11on = true;
bool PR = true;
bool LEDS = true;
bool HVACO = true;
bool DCMotor = true;

//pins for leds
int greenLed = 4;
int blueLed = 3;

//configuration for stepper motor
int const rotation = 2048;
Stepper stepperS(rotation, 8, 10, 9, 11);

//configuration for DC motor
Servo servoS;
int servoPin = 6;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);

  dht.begin();

  pinMode(greenLed, OUTPUT);
  pinMode(blueLed, OUTPUT);

  servoS.attach(servoPin);
  servoS.write(90);

  stepperS.setSpeed(10);
}

void loop() {
  
    //variables to read temperature,humidity and light
      float  h = dht.readHumidity();
      float  t = dht.readTemperature();
      int  Pmeasure = analogRead(photoResistor);

  if (Serial.available() > 0) {
    char command = Serial.read();


    //all commands to turn on/turn off all equipments connected to Arduino
    if (command == 'D') {
      DHT11on = false;
    }

    if (command == 'd') {
      DHT11on = true;
    }

    if (command == 'P') {
      PR = false;
    }

    if (command == 'p') {
      PR = true;
    }

    if (command == 'H') {
      HVACO = false;
    }

    if (command == 'h') {
      HVACO = true;
    }

    if (command == 'L') {
      LEDS = false;
    }

    if (command == 'l') {
      LEDS = true;
    }

    if (command == 'S') {
      DCMotor = false;
    } 

    if (command == 's') {
      DCMotor = true;
    }   

    //command to measure in Arduino IDE and especially in C# app
    if (command == 'M') {
      if (DHT11on == true && PR == true) {

        h = dht.readHumidity();
        t = dht.readTemperature();
        Pmeasure = analogRead(photoResistor);

        //print mode in Arduino IDE,and C# app
        Serial.print(t);
        Serial.print("|");
        Serial.print(h);
        Serial.print("|");
        Serial.println(Pmeasure);

      } else if (DHT11on == true && PR == false) {

        h = dht.readHumidity();
        t = dht.readTemperature();
        Serial.print(t);
        Serial.print("|");
        Serial.print(h);
        Serial.print("|");
        Serial.println("nan");

      } else if (DHT11on == false && PR == true) {

        Pmeasure = analogRead(photoResistor);
        Serial.print("nan|");
        Serial.print("nan|");
        Serial.println(Pmeasure);

      }else {

      Serial.print("nan|");
      Serial.print("nan|");
      Serial.println("nan");

      }

    }


    //conditions to turn on leds
    if(LEDS == true){
      if (Pmeasure < 300) {
        digitalWrite(greenLed, HIGH);
        digitalWrite(blueLed, LOW);
      } else if (Pmeasure < 500) {
        digitalWrite(blueLed, HIGH);
        digitalWrite(greenLed, HIGH);
      } else {
        digitalWrite(blueLed, LOW);
        digitalWrite(greenLed, LOW);
      }
    }

    //conditions to turn on Stepper motor
    if(HVACO == true){
       if (t > 28) {
          stepperS.setSpeed(5);
          stepperS.step(double(rotation / 4));
       } else if (t > 25) {
          stepperS.setSpeed(8);
          stepperS.step(double(rotation / 2));
       } else if (t > 20) {
        stepperS.setSpeed(12);
        stepperS.step(double(rotation));
      }
    }

    //conditions to turn on DC motor
    if(DCMotor == true){
      if (h > 70) {
        servoS.write(180);
      } else if (h > 60) {
        servoS.write(155);
      } else if (h > 50) {
        servoS.write(120);
      } else if (h > 45) {
        servoS.write(100);
      } else {
        servoS.write(90);
      }
    }

  }
}

