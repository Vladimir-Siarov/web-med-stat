// https://docs.mongodb.com/manual/tutorial/enable-authentication/

use admin
db.createUser(
  {
    user: "admin",
    pwd: passwordPrompt(), // or cleartext password
    roles: [ { role: "userAdminAnyDatabase", db: "admin" }, "readWriteAnyDatabase" ]
  }
)

/*
Re-start the MongoDB instance with access control.
*/

// Authenticate after Connection
use admin
db.auth("admin", passwordPrompt()) // or cleartext password

// Create Logger user for MedStat.WebAdmin project
use medStat-webAdmin-log
db.createUser(
  {
    user: "serilogUser",
    pwd:  passwordPrompt(),   // or cleartext password
    roles: [ { role: "readWrite", db: "medStat-webAdmin-log" } ]
  }
)

// Connect by created user
// mongo --port 27017 -u "serilogUser" --authenticationDatabase "medStat-webAdmin-log" -p

// Connection string
// mongodb://serilogUser:***@localhost:27017/medStat-webAdmin-log?authSource=medStat-webAdmin-log&readPreference=primary&appname=webadmin&ssl=false
// NOTE: Check connection string in Compass before usage, because Serilog / MongoDB doesn't display any notifications for auth error.