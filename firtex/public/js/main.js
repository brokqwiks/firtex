const menu_btn = document.getElementById('button__menu');
const profile_menu = document.querySelector('.profile-menu');
const menu = document.querySelector('.menu');

var open_menu = false;

window.onload = function()
{
    menu.style.left = "7%";
}


menu_btn.onclick = function()
{   
    if(open_menu == false){
        profile_menu.style.transform = 'translate(0,0)';
        open_menu = true;
    }
    else
    {
        profile_menu.style.transform = 'translate(-100%,0)';
        open_menu = false;
    }
}