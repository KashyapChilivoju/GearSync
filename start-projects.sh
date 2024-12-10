#!/bin/bash

# Define the API port
API_PORT=5187

# Kill any process using the API port
echo "Checking for processes on port $API_PORT..."
PID=$(lsof -ti:$API_PORT)
if [ -n "$PID" ]; then
  echo "Killing process $PID on port $API_PORT..."
  kill -9 $PID
fi

# Start the API
echo "Starting API..."
cd GearSyncAPI || exit
dotnet run &

# Start the UI
echo "Starting UI..."
cd ../GearSyncUI || exit
npm install
npm run dev
