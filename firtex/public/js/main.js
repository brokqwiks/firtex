const menu_btn = document.querySelector('#button__menu');
const profile_menu = document.querySelector('.profile-menu');
const menu_list_btn = document.querySelector('.menu__button');
const menu = document.querySelector('.menu');
const create_wallet_bool = document.querySelector('#create__wallet-bool');
const wallet_create_block = document.querySelector('.wallet-create-block');
const private_key_active = document.querySelector('.wallet-private-key_active')
const show_private_key_btn = document.querySelector('.button2');
const hidden_private_key = document.getElementsByClassName('hidden-private-key-block');
const private_key_el = document.getElementsByClassName('private-key-el');
const copy_btn = document.querySelector('.Btn');
const private_key_str = document.getElementById('private-key-str');
const active_menu = document.querySelector('.active-menu');
const active_menu_text = document.querySelector('#active-menu-text');

var open_menu = false;

window.onload = function()
{
    menu.style.left = "7%";
}


menu_btn.onclick = function()
{   
    if(open_menu == false){
        menu_btn.style.transform = "translate(-430%,0)"
        profile_menu.style.transform = 'translate(0,0)';

        open_menu = true;
    }
    else
    {   
        menu_btn.style.transform = "translate(0,0)"
        profile_menu.style.transform = 'translate(-100%,0)';
        open_menu = false;
    }
}

const sleep = (milliseconds) => {
    return new Promise(resolve => setTimeout(resolve, milliseconds))
}


loading_chars = ['.', '..', '...'];

function funct(){
    const Loading = (el, text, len = 3, time = 500) => {
        let i = 0;
        setInterval(() => {
            if(i > len) i = 0;
            el.innerHTML = text+'.'.repeat(i++);
        }, time);
    }
    Loading(document.querySelector('#create-h1'), 'Creating your wallet', 3, 500);
}
funct()


 
if (create_wallet_bool.value == "true") {
    setTimeout(() => {wallet_create_block.style.visibility = "hidden"; private_key_active.style.visibility = "visible"}, 6000);
}

var private_key_btn = false

show_private_key_btn.addEventListener('click', function()
{   
    if(private_key_btn == false){
    for (let index = 0; index < hidden_private_key.length; index++) {
        const element = hidden_private_key[index];
        element.style.visibility = "hidden";
    }
    for (let index = 0; index < private_key_el.length; index++) {
        const element = private_key_el[index];
        element.style.visibility = "visible"
    }

    show_private_key_btn.innerHTML = "Hide"

    copy_btn.style.visibility = "visible";
    private_key_btn = true
    }
    else
    {
        for (let index = 0; index < hidden_private_key.length; index++) {
            const element = hidden_private_key[index];
            element.style.visibility = "visible";
        }
        for (let index = 0; index < private_key_el.length; index++) {
            const element = private_key_el[index];
            element.style.visibility = "hidden"
        }

        show_private_key_btn.innerHTML = "Show"

        copy_btn.style.visibility = "hidden";
        private_key_btn = false;
    }
})

const animation_checkmark = document.querySelector('.checkmark')



copy_btn.addEventListener('click', function()
{   
    const private_key = private_key_str.value;
    
    const words = private_key.split(" ");
    const modifiedSentence = words.map((word, index) => `${index + 1 + '.'} ${word}`);
    
    const result = modifiedSentence.join(" ");
    navigator.clipboard.writeText(result);
    
    const next_btn = document.querySelector('.next-btn');
    next_btn.style.visibility = "visible";
    next_btn.style.left = "70%";
    
    document.documentElement.scrollIntoView(true);
    
    active_menu_text.innerHTML = "Copied";
    active_menu.style.width = "230px";
    active_menu.style.left = "44%"
    active_menu_text.style.left = "42%";
    active_menu.style.top = "5%";
    setTimeout(() => {active_menu.style.top = "-100%";}, 3000)
});

const next_btn = document.querySelector('.next-btn');
const confirm_private_key = document.getElementsByClassName('confirm-private-key');
const send_btn = document.querySelector('.send-btn');

next_btn.onclick = function() {
    next_btn.style.visibility = "hidden";
    show_private_key_btn.style.visibility = "hidden"
    copy_btn.style.visibility = "hidden";
    send_btn.style.visibility = "visible";
    for (let index = 0; index < private_key_el.length; index++) {
        const element = private_key_el[index];
        element.style.visibility = "hidden"
    }
    for (let index = 0; index < hidden_private_key.length; index++) {
        const element = hidden_private_key[index];
        element.style.visibility = "hidden";
    }
    for (let index = 0; index < confirm_private_key.length; index++) {
        const element = confirm_private_key[index];
        element.style.visibility = "visible";
    }
}

const succes_private_key = document.getElementById('succes')


