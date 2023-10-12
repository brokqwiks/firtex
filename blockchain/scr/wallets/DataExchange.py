import socket

class DataExchange():
    def data_handler(self):

        while True:
            server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            server.bind(('127.0.0.1', 7000))
            server.listen(4)
            client_socket, adress = server.accept()
            data_connection = client_socket.recv(1024).decode('utf-8')
            content = "reconnect...".encode('utf-8')
            client_socket.send(content)

            data_string = data_connection.split('\n')
            data_string = data_string[len(data_string) - 1]

            elements = data_string.split("&")

            data = {}

            for element in elements:
                key, value = element.split("=")
                data[key] = value

            return data