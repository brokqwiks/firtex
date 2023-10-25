import time

import DataExchange, WalletDesigner, WalletConnector
from threading import Thread

class Handler():

    def create_wallet(self):
        exchange = DataExchange.DataExchange
        wallet = WalletDesigner.WalletDesigner
        connector = WalletConnector.WalletConnector
        creating_wallet = exchange.create_wallet_data_handler(1)
        if creating_wallet[0] == 'create_wallet':
                private_key = wallet.create_private_key(1)
                public_key = wallet.private_key_to_public_key(private_key[0])[0]
                connector.send_private_key(private_key, creating_wallet[1])
                time.sleep(3)
                return connector.delete_private_key(1)

while True:
    Handler.create_wallet(1)