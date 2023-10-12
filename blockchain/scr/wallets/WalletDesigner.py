from hashlib import *
from time import time, ctime
from ecdsa import SigningKey, SECP256k1
from cryptography.hazmat.primitives.asymmetric import ec
from near_seed_phrase.main import generate_seed_phrase
from Blockthon import Wallet

class WalletDesigner:

    def create_private_key(self):
        private_key = Wallet.getPrivateKey()
        seed_phrase = Wallet.PrivateKey_To_Mnemonic(private_key, size=12)

        return [private_key, seed_phrase]

    def private_key_to_bytes(private_key):
        byte = Wallet.PrivateKey_To_Bytes(private_key)

        return byte

    def private_key_to_binary(private_key):
        binary_string = Wallet.PrivateKey_To_Binary(private_key)

        return binary_string

    def private_key_to_public_key(private_key):
        public_key_uncompress = Wallet.PrivateKey_To_PublicKey(private_key)
        public_key_compress = Wallet.PrivateKey_To_PublicKey(private_key, compress=True)


        return [public_key_compress, public_key_uncompress]

    def get_adress(private_key):
        address_uncompress = Wallet.PrivateKey_To_Address(private_key, compress=False)
        address_compress = Wallet.PrivateKey_To_Address(private_key, compress=True)

        return [address_compress, address_uncompress]

