const menu_btn = document.querySelector('#button__menu');
const profile_menu = document.querySelector('.profile-menu');
const menu_list_btn = document.querySelector('.menu__button');
const menu = document.querySelector('.menu');

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
    Loading(document.querySelector('#create-h1'), 'Create your wallet', 3, 500);
}
funct()