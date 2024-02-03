using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VideojuegoProyectoCliente
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using (ClientWebSocket clientWebSocket = new ClientWebSocket())
            {
                await clientWebSocket.ConnectAsync(new Uri("ws://localhost:1600"), CancellationToken.None);

                Console.WriteLine("Conectado al servidor WebSocket.");

                // Iniciar un bucle para recibir actualizaciones del servidor
                _ = Task.Run(async () =>
                {
                    while (true)
                    {
                        ArraySegment<byte> receiveBuffer = new ArraySegment<byte>(new byte[1024]);
                        WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(receiveBuffer, CancellationToken.None);
                        string receivedMessage = Encoding.UTF8.GetString(receiveBuffer.Array, 0, result.Count);
                        Console.WriteLine($"Actualización del servidor: {receivedMessage}");

                        await Task.Delay(2000); // Esperar 4 segundos antes de la próxima actualización
                    }
                });

                // Bucle principal para enviar solicitudes al servidor
                while (true)
                {
                    Console.Write("\u001b[33mIngrese 'juego' para iniciar a jugar: \u001b[0m\n");
                    string input = Console.ReadLine();

                    if (input.ToLower() == "exit")
                        break;

                    if (input != "")
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(input);
                        await clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                        // No necesitamos esperar respuesta explícitamente aquí
                    }
                    else
                    {
                        Console.WriteLine("Entrada no válida. Por favor, ingrese un número de piso válido.");
                    }
                }

                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Cerrando conexión", CancellationToken.None);
            }
        }
    }
}
