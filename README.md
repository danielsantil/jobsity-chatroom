# Jobsity Chatroom

### Instructions
- Download project as zip or clone this repository using git
- Download and install [Docker](https://www.docker.com/products/docker-desktop)
- Open a terminal in the project folder
- Run the following command `docker-compose up` to start the services. This might take some time, be patient.
    - If you wish to reun the services in background, run `docker-compose up -d`.
- Open a browser and go to *http://localhost:8080*

**Note about browsers support**: As long as you're not using IE10 or older, you should be fine. :)

To terminate everything, simply close the terminal. If you executed the services in background mode, run `docker-compose down` in a terminal inside the project folder.

### Stack
- .NET Core 3.1
    - Entity Framework
    - .NET Identity
    - CsvHelper
- MS SQL Server
- RabbitMQ
- Angular 11
    - Bootstrap 4
- SignalR: For handling Websockets
- xUnit: Unit tests
- Docker: containers

### Requirements
- ✅ Allow registered users to log in and talk with other users in a chatroom.
- ✅ Allow users to post messages as commands into the chatroom with the following format /stock=stock_code
- ✅ Create a decoupled bot that will call an API using the stock_code as a parameter (https://stooq.com/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv, here aapl.us is the stock_code)
- ✅ The bot should parse the received CSV file and then it should send a message back into the chatroom using a message broker like RabbitMQ. The message will be a stock quote using the following format: “APPL.US quote is $93.42 per share”. The post owner will be the bot.
- ✅ Have the chat messages ordered by their timestamps and show only the last 50 messages.
- ✅ Unit test the functionality you prefer.

### Bonus
- ✅ Use .NET identity for users authentication
- ✅ Handle messages that are not understood or any exceptions raised within the bot.
- ✅ Build an installer (through Docker)

### Additional notes

In case you want to, you can also access the database and MQ management portal:
- MS SQL Server:
    - *Server*: localhost,1433
    - *User*: sa
    - *Password*: Passw0rd

- RabbitMQ:
    - *Server*: http://localhost:15672
    - *User*: admin
    - *Password*: admin
