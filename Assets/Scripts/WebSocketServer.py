import asyncio
import websockets

async def server(websocket, path):

    print("Ein Unity-Client hat sich verbunden!")

    try:
        async for message in websocket:
            if isinstance(message, bytes):
                print("Binärnachricht empfangen:", message)
                # Binäre Nachricht hier verarbeiten von NativeWebSocket
            else:
                print("Textnachricht empfangen:", message)
                # Verarbeitung der Textnachricht von NativeWebSocket

            # Echo zurücksenden
            response = "Echo: " + str(message)
            await websocket.send(response)

    except websockets.exceptions.ConnectionClosed as e:
        print(f"Verbindung wurde geschlossen: {e}")


start_server = websockets.serve(server, "localhost", 2567)

asyncio.get_event_loop().run_until_complete(start_server)

print("WebSocket-Server läuft unter: ws://localhost:2567")

asyncio.get_event_loop().run_forever()
