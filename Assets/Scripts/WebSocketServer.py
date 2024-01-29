import asyncio
import websockets
import json

async def server(websocket, path):

    print("A Unity client has connected!")

    try:
        async for message in websocket:
             # Parse JSON msg object.
            data = json.loads(message)
            print("Received Data:", data)

            # Accessing to the data fields and sending responses to Unity
            for key, value in data.items():

                response = f"Echo {key}: {value}"

                await websocket.send(response)

                print("Sent data (to Unity):", response)

    except websockets.exceptions.ConnectionClosed as e:
        print(f"Connection was closed: {e}")


start_server = websockets.serve(server, "localhost", 2567)

asyncio.get_event_loop().run_until_complete(start_server)

print("WebSocket-Server runs at: ws://localhost:2567")

asyncio.get_event_loop().run_forever()
