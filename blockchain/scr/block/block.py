import json
import hashlib

class Block:

    def create_block(id, hash_last_block, method):
        import json

        # Открываем JSON-файл для чтения
        with open('C:\\OpenServer\\domains\\firtex\\blockchain\\data\\blockchain.json', 'r+') as f:
            # Загружаем существующий JSON-объект или создаем новый
            try:
                data = json.load(f)
            except json.decoder.JSONDecodeError:
                data = {}

            # Добавляем новую пару ключ-значение в JSON-объект
            data['block_{}'.format(id)] = {
                "id": id,
                "hash_last_block": hash_last_block,
                "method": method
            }

            # Перематываем файл в начало и записываем измененный JSON-объект
            f.seek(0)
            json.dump(data, f, indent=4)

            # Усекаем файл до актуального размера, если новый JSON-объект был меньше предыдущего
            f.truncate()


    def create_hash_block(json_file):
        with open("C:/OpenServer/domains/firtex/blockchain/data/blockchain.json", 'r') as file:
            data = json.load(file)

        # Получение значений из блока с ключом "block_0"
        block_0 = data.get('block_0')

        # Преобразование значений блока в строку для хэширования
        block_string = json.dumps(block_0, sort_keys=True)
        # Примечание: добавлен sort_keys=True, чтобы гарантировать, что порядок ключей одинаков для каждого хэширования

        # Хэширование строки блока с помощью SHA-256
        hash_object = hashlib.sha256(block_string.encode())
        hash_value = hash_object.hexdigest()
        print(hash_value)

Block.create_hash_block(1)