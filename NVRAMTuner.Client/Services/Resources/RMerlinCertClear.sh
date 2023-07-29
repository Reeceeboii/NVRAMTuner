#!/bin/sh

# THIS SCRIPT WAS TAKEN FROM https://www.snbforums.com/threads/asus-rt-ac68u-386-2_6-low-on-free-nvram.73158/post-695283
# It is licensed according to the wider SNBForums Ts&Cs: https://www.snbforums.com/help/terms/

echo "Removing unused cert/key from nvram..."

for i in 1 2 3 4 5
do
    nvram unset vpn_crt_client$i\_ca
    nvram unset vpn_crt_client$i\_extra
    nvram unset vpn_crt_client$i\_crt
    nvram unset vpn_crt_client$i\_key
    nvram unset vpn_crt_client$i\_crl
    nvram unset vpn_crt_client$i\_static
done

for i in 1 2
do
    nvram unset vpn_crt_server$i\_ca
    nvram unset vpn_crt_server$i\_dh
    nvram unset vpn_crt_server$i\_ca_key
    nvram unset vpn_crt_server$i\_extra
    nvram unset vpn_crt_server$i\_client_crt
    nvram unset vpn_crt_server$i\_crl
    nvram unset vpn_crt_server$i\_crt
    nvram unset vpn_crt_server$i\_key
    nvram unset vpn_crt_server$i\_static
    nvram unset vpn_crt_server$i\_client_key
done

# SSH also migrated host keys to jffs a while back
nvram unset sshd_dsskey
nvram unset sshd_ecdsakey
nvram unset sshd_hostkey

nvram commit

echo "done."