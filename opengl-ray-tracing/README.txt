2002 openGL - Ray Tracing

The program is a simple implementation of the Ray Tracing technique. Though nowhere near POVRay we can still write simple .ray files and get good rendering. Objects such as the sphere and the plane etc are recognized. Complex scenes can also be created as demonstrated by our example. Ray tracing is a slow process requiring powerful computers. 

Primitives Available:
Cuboid (Box)
Sphere
Plane
Ellipsoid

Features:
Multiple Light Sources (Point, Parallel, Spot)
Shadows
Depth of rays traced
Different viewing angles
Whitted Equation for global illumination

File Outputs: The samples provided can be rendered as required and in addition our own scenes have also been beautifully rendered. 

Implementation: The skeleton code was already provided so we have extend a few methods, these included methods to find the intersections between rays and objects, read objects from .ray file, calculate illumination effects using the Whitted equation, calculate shadow effects, calculate refraction   and reflection of rays and multiple reflections/refractions/transmissions of a ray.

Program Usage:
The interface is simple and easy to use. The steps to follow are:
1. Open .ray file
2. Render scene
3. View the rendered file
4. Quit the program