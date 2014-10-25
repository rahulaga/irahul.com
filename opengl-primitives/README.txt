2002 - openGL - Drawing Primitives

The program implements a 3D Modeling Program using primitive 3D objects such as cube, sphere etc. and creates a user interface to modify these        objects. User interacts through the use of a pointing device and the keyboard. 3D Environments created can and saved and modified later. The program is interactive and user friendly

Primitives Available:
Cube
Cuboid
Cone
Sphere
Torus
Pyramid
Tetrahedron
Teapot

Features:
3D Animation
Rotation
Translation
Scaling
Colors
Wire Frame/Solid Objects
Zoom
Saving/Loading Environment
Full Screen Mode
Help

Special Features:
Separate Colors for six planes with single click n pick coloring palettes
Lighting/Shading Effects


Sample Screen Shot:This shows how the program looks. The first two columns show the 4 viewing locations of the environment. These are the Front, Left, Top and 3D Views. The third column contains the active or selected object and six RGBA color palettes, one for each plane. For example in a             cube, each of the six surfaces can be assigned a unique color. The user interacts in the 3D View window and these changes are immediately           rendered in all the windows.


Implementation:For program implementation main consideration was being able to maintain the state of each object separately. This was achieved by defining the objects using struct and using arrays of objects to hold the various objects in the windows. For the saving/loading of the environment, the whole array of struct is written into the file as one single structure. This ensures that the saving and loading is faster than using text        files. However the main problem was changing the camera view using gluLookAt. To overcome this difficulty the camera was simulated to move in a polygon around the origin. 

Special Features:
6 Plane Click n Pick Coloring: This intuitive 6 surface RGBA color palette helps change colors uniquely through a single click with the pointing device one the palette corresponding to the surface. The RGBA values are calculated based on the distance of the point of click on the palette from the four corners that represent the Red, Green, Blue and Alpha. With Objects having only one surface (sphere etc.) only the first surface applies, similarly for other figures with fewer planes.

Lighting Effects: With lighting effects enabled, 3D objects such as sphere, cone, torus and teapot appear as 3D objects. In the absence of lighting effects these would appear flat and lifeless. The glLight* function of the OpenGL API is used to setup a fixed light in the 3D environment.

Program Usage: The interface is simple and easy to use. The menus are activated by the right click of the mouse and are intuitive.

Menu Hierarchy
Inserting Objects: Choose desired object from the 'Insert' sub menu. All the following act on the Active Object which is shown and all interaction is done in the 3D view window. You can cycle through existing objects by the TAB key.
Deletion: The object is deleted.
Scaling: From the 'Size' sub menu choose the axis you want to scale along and then a corresponding value for it.
Surface Selection: Choose between Wire Frame and Solid objects from the 'Surfaces' sub menu.
Translate: After clicking translate, click anywhere in the window to translate the object there.
Rotation: Click Rotate On and then click and drag the mouse to rotate. Rotate Off turns off the feature.
Zoom: Zoom In or Out 
Animation: Begins 3D animation
Load/Save: Choose desired file to Load and save
Help: Online Help provides all keyboard commands.
Quit: Closes program