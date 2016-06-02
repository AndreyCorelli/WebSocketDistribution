set src=..\WebSocketDistribution\bin\Debug
set dst=lib

robocopy %src% %dst% NetSockets.dll /S
robocopy %src% %dst% SuperSocket.Common.dll /S
robocopy %src% %dst% SuperSocket.Dlr.dll /S
robocopy %src% %dst% SuperSocket.Facility.dll /S
robocopy %src% %dst% SuperSocket.SocketBase.dll /S
robocopy %src% %dst% SuperSocket.SocketEngine.dll /S
robocopy %src% %dst% SuperWebSocket.dll /S
robocopy %src% %dst% WebSocket4Net.dll /S
robocopy %src% %dst% WebSocketDistribution.dll /S

pause