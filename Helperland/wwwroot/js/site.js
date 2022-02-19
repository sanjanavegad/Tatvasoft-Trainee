// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// all pages js for sidebar
$(document).ready(function () {

    $('#dismiss, .overlay').on('click', function () {
        // hide sidebar
        $('#sidebar').removeClass('active');
        // hide overlay
        $('.overlay').removeClass('active');
    });

    $('#sidebarCollapse').on('click', function () {
        // open sidebar
        $('#sidebar').addClass('active');
        // fade in the overlay
        $('.overlay').addClass('active');
        $('.collapse.in').toggleClass('in');
        $('a[aria-expanded=true]').attr('aria-expanded', 'false');
    });
});

// FAQ page thumb-right
$("#accordion").on("hide.bs.collapse show.bs.collapse", e => {
    $(e.target)
        .prev()
        .find("img:last-child")
        .toggleClass("right rotated");
});
$("#accordion2").on("hide.bs.collapse show.bs.collapse", e => {
    $(e.target)
        .prev()
        .find("img:last-child")
        .toggleClass("right rotated");
});

// All pages Change navbar
window.onscroll = function () { scrollFunction() };
function scrollFunction() {
    if (document.body.scrollTop > 80 || document.documentElement.scrollTop > 80) {
        document.getElementById("navbar").style.backgroundColor = "rgba(19, 19, 19, 0.8)";
    }
}
// Write your JavaScript code.


/*currunt Date and time*/
   
        var date = new Date();
        var currentDate = date.toISOString().slice(0, 10);
        var currentTime = date.getHours() + ':' + date.getMinutes();
        document.getElementById('currentDate').value = currentDate;
        document.getElementById('currentTime').value = currentTime;
 

   
 ///* <!--move to next button-- >*/
 //       form = document.getElementById('Form');
 //       fieldsets = document.querySelectorAll('fieldset');
 //       back = document.getElementById('back');
 //       back1 = document.getElementById('back1');
 //       nextORsubmit = document.getElementById('nextORsubmit');



 //       let i = 0;

 //       form.addEventListener('submit', (e) => {

 //           if (i < fieldsets.length - 1) {
 //               e.preventDefault();

 //               console.log('Validate, but don\'t send form');
 //               fieldsets[i].style.display = 'none';
 //               fieldsets[i + 1].style.display = 'block';
 //               back.style.display = 'inline';
 //               $("#progressbar li").eq($("fieldset").index(fieldsets[i + 1])).addClass("active");
 //               i++;



 //               // set required on current fieldset inputs, except if they're checkboxes
 //               fieldsets[i].querySelectorAll('input:not([type="checkbox"])').forEach(el => {
 //                   el.required = true;
 //               })
 //           }
 //       });

 //       back.addEventListener('click', () => {
 //           console.log('going back a step');
 //           fieldsets[i].style.display = 'none';
 //           fieldsets[i - 1].style.display = 'block';
 //           $("#progressbar li").eq($("fieldset").index(fieldsets[i])).removeClass("active");
 //           i--;

 //           // remove required on inputs from the next fieldset that we've just hid
 //           fieldsets[i + 1].querySelectorAll('input:not([type="checkbox"])').forEach(el => {
 //               el.required = false;
 //           })

 //           // remove back button when you go back to the first step
 //           if (i == 0) {
 //               back.style.display = 'none';
 //           }
 //       })
 //       back1.addEventListener('click', () => {
 //           console.log('going back a step');
 //           fieldsets[i].style.display = 'none';
 //           fieldsets[i - 1].style.display = 'block';
 //           $("#progressbar li").eq($("fieldset").index(fieldsets[i])).removeClass("active");
 //           i--;

 //           // remove required on inputs from the next fieldset that we've just hid
 //           fieldsets[i + 1].querySelectorAll('input:not([type="checkbox"])').forEach(el => {
 //               el.required = false;
 //           })

 //           // remove back button when you go back to the first step
 //           if (i == 0) {
 //               back1.style.display = 'none';
 //           }
 //       })

  
 //   /*<!--move to next button-- >*/


 //       $(document).ready(function () {
 //           $('.btn1').click(function () {
 //               $(this).addClass('active');
 //           });
 //       });
 