import DataExchange, WalletDesigner, WalletConnector
from threading import Thread

class Handler():

    def create_wallet(self):
        exchange = DataExchange.DataExchange
        wallet = WalletDesigner.WalletDesigner
        connector = WalletConnector.WalletConnector
        while True:
            creating_wallet = exchange.create_wallet_data_handler(1)
            delete_data = exchange.delete_data_handler(1)
            if creating_wallet[0] == 'create_wallet':
                private_key = wallet.create_private_key(1)
                public_key = wallet.private_key_to_public_key(private_key[0])[0]
                return connector.send_private_key(private_key, creating_wallet[1])
            elif delete_data[0] == "delete_data":
                return connector.delete_private_key(1)

while True:
    Handler.create_wallet(1)