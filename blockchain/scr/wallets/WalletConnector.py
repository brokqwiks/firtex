from WalletDesigner import WalletDesigner
from DataExchange import DataExchange
import pymysql
import requests


class WalletConnector():
    def send_private_key(data, login):
        file = open("C:/OpenServer/domains/test/blockchain/data/data.txt", "a")
        file.write(f'{login}\n')
        file.write(f'{data[0]}\n')
        file.write(f'{data[1]}\n\n')
        file.close()
        return data

    def delete_private_key(self):
        file = open("C:/OpenServer/domains/test/blockchain/data/data.txt", "a")
        file.seek(0)
        file.truncate()
        file.close()
        return 'succes'

WalletConnector.delete_private_key(1)
