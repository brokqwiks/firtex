from WalletDesigner import WalletDesigner
import pymysql


class WalletConnector():
    def send_private_key(data):
        file = open("C:/OpenServer/domains/test/blockchain/data/data.txt", "a")
        file.write(f'\n{data[0]}\n')
        file.write(f'{data[1]}\n')
        file.close()
        return data




WalletConnector.send_private_key(WalletDesigner.create_private_key(1))