import socket

class DataExchange():
    def create_wallet_data_handler(self):

            server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            server.bind(('127.0.0.1', 7000))
            server.listen(4)
            client_socket, adress = server.accept()
            data_connection = client_socket.recv(1024).decode('utf-8')
            content = "reconnect...".encode('utf-8')
            client_socket.send(content)

            lines = data_connection.split('\n')

            # Создаем флаги для поиска данных после строк 'name="0"' и 'name="1"'
            found_0_flag = False
            found_1_flag = False
            response_0 = ""
            response_1 = ""

            # Ищем строки с 'name="0"' и 'name="1"', а затем извлекаем следующие непустую строку
            for line in lines:
                if 'name="0"' in line:
                    found_0_flag = True
                elif 'name="1"' in line:
                    found_1_flag = True
                elif found_0_flag and line.strip():
                    response_0 = line.strip()
                    found_0_flag = False  # сбрасываем флаг после нахождения желаемой строки
                elif found_1_flag and line.strip():
                    response_1 = line.strip()
                    found_1_flag = False  # сбрасываем флаг после нахождения желаемой строки

            return print(data_connection)
    def delete_data_handler(self):
        server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        server.bind(('127.0.0.1', 7100))
        server.listen(4)
        client_socket, adress = server.accept()
        data_connection = client_socket.recv(1024).decode('utf-8')
        content = "reconnect...".encode('utf-8')
        client_socket.send(content)

        lines = data_connection.split('\n')

        # Создаем флаги для поиска данных после строк 'name="0"' и 'name="1"'
        found_0_flag = False
        found_1_flag = False
        response_0 = ""
        response_1 = ""

        # Ищем строки с 'name="0"' и 'name="1"', а затем извлекаем следующие непустую строку
        for line in lines:
            if 'name="0"' in line:
                found_0_flag = True
            elif 'name="1"' in line:
                found_1_flag = True
            elif found_0_flag and line.strip():
                response_0 = line.strip()
                found_0_flag = False  # сбрасываем флаг после нахождения желаемой строки
            elif found_1_flag and line.strip():
                response_1 = line.strip()
                found_1_flag = False  # сбрасываем флаг после нахождения желаемой строки

        return print([response_0,response_1])

DataExchange.delete_data_handler(1)

