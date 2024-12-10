# Start the API
echo "Starting API..."
cd GearSyncAPI || exit
dotnet run &

# Start the UI
echo "Starting UI..."
cd ../GearSyncUI || exit
npm install
npm run dev
