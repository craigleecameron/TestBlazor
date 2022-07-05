//scroll if at the bottom of the page
window.onscroll = function () {
    if (window.scrollInfoService != null)
        window.scrollInfoService.invokeMethodAsync('OnScroll', window.pageYOffset + window.innerHeight - window.document.documentElement.offsetHeight);
}

window.RegisterScrollInfoService = (scrollInfoService) => {
    window.scrollInfoService = scrollInfoService;
}