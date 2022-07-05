Just a few notes on this website.  It was written using Blazor.  The "technical" decision behind choosing blazor was that I had never tried using it before and I wanted to see what it was like.

I honestly don't forsee myself choosing Blazor in the short term especially for higher volume sites but it was fun to learn a bit about it.
       
For the most part much of this code has been automatically generated when you setup a new blazor site in Visual Studio.
        
The markup and styling has been kept to an absolute minimum.
        
I have also tried to uses a few third party libraries as possible as I believe this can impact maintainability.
        
Instead of a Carousel I decided I would attempt to make a load more on scroll to the bottom of the page.  This as I later found out was a bit convoluted to do in Blazor as it doesn't really have an elegant way of determining if you are at the bottom of the page.  Blazor is marketed as being able to do it all without JavaScript however this isn't exactly the case.  So you will see a bit of a work around here where you have Javascript which registers an onscroll event back to a handler.
        
As a side note I am not retrieving the HTML from a web request and then looking for images.  I am just looking for &lt;img tags in the returned HTML.  I am not doing any of the following: fully parsing the css to find what images might be set as backgrounds and returning them in the list, not checking iframes, not looking at how javascript contrive to create images on the fly
