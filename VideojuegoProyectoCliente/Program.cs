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
                string receivedMessage = "inicio";

                // Iniciar un bucle para recibir actualizaciones del servidor
                _ = Task.Run(async () =>
                {
                    while (true)
                    {
                        ArraySegment<byte> receiveBuffer = new ArraySegment<byte>(new byte[1024]);
                        WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(receiveBuffer, CancellationToken.None);
                        receivedMessage = Encoding.UTF8.GetString(receiveBuffer.Array, 0, result.Count);
                        Console.WriteLine($"Actualización del servidor: {receivedMessage}");

                        await Task.Delay(100); // Esperar 1 segundos antes de la próxima actualización
                    }
                });

                // Bucle principal para enviar solicitudes al servidor

                while (true)
                {
                    
                    await Task.Delay(1000);
                    string input ="";
                    if (receivedMessage == "\u001b[36mMundos Disponibles: Jimwestt y Kethan. Escoja uno:\u001b[0m")
                    {
                        Console.Write($"\u001b[33mEscoja el mundo: \u001b[0m\n");
                        input = Console.ReadLine();
                    }
                    else if (receivedMessage == "inicio")
                    {
                        Console.Write($"\u001b[33mIngrese 'iniciar juego' para iniciar a jugar: \u001b[0m\n");
                        input = Console.ReadLine();
                    }
                    else if (receivedMessage == "\u001b[36mSe ha elegido Jimwestt.\u001b[0m" || receivedMessage == "\u001b[36mSe ha elegido Kethan.\u001b[0m")
                    {
                        Console.Write($"\u001b[33mRecogiendo Objetos... \u001b[0m\n");
                        await Task.Delay(1000);
                        Console.Write($"\u001b[33mRecogiendo Objetos... \u001b[0m\n");
                        await Task.Delay(500);
                        Console.Write($"\u001b[33mRecogiendo Objetos... \u001b[0m\n");

                        await Task.Delay(3000);
                        Console.Write($"\u001b[33mQueda Poco Tiempo \u001b[0m\n");
                        await Task.Delay(1500);
                        Console.Write($"\u001b[33mTiempo Agotado \u001b[0m\n");
                        await Task.Delay(3000);
                        Console.Write($"\u001b[33mCalculando Resultados... \u001b[0m\n");
                        await Task.Delay(3500);
                        Console.Write($"\u001b[33mHas Obtenido la Mayor Cantidad de Objetos, ¡eres el ganador! \u001b[0m\n");
                        await Task.Delay(3000);
                        Console.Write($"\u001b[33mEl juego ha terminado \u001b[0m\n");
                        await Task.Delay(1500);
                        input = "exit";
                    }

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
                        Console.WriteLine("Entrada no válida. Por favor, ingrese un valor valido.");
                    }
                }

                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Cerrando conexión", CancellationToken.None);
            }
        }
    }
}
