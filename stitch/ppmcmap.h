/* ppmcmap.h - header file for colormap routines in libppm
*/

/* Color histogram stuff. */

typedef struct colorhist_item* colorhist_vector;
struct colorhist_item
    {
    pixel color;
    int value;
    };

typedef struct colorhist_list_item* colorhist_list;
struct colorhist_list_item
    {
    struct colorhist_item ch;
    colorhist_list next;
    };

colorhist_vector
ppm_computecolorhist( pixel ** const pixels, 
                      const int cols, const int rows, const int maxcolors, 
                      int * const colorsP );
/* Returns a colorhist *colorsP long (with space allocated for maxcolors. */

void
ppm_addtocolorhist( colorhist_vector chv, 
                    int * const colorsP, const int maxcolors, 
                    const pixel * const colorP, 
                    const int value, const int position );

void
ppm_freecolorhist( colorhist_vector chv );


/* Color hash table stuff. */

typedef colorhist_list* colorhash_table;

colorhash_table
ppm_computecolorhash( pixel ** const pixels, 
                      const int cols, const int rows, 
                      const int maxcolors, int * const colorsP );

int
ppm_lookupcolor( const colorhash_table cht, const pixel * const colorP );

colorhist_vector
ppm_colorhashtocolorhist( const colorhash_table cht, const int maxcolors );

colorhash_table
ppm_colorhisttocolorhash( const colorhist_vector chv, const int colors );

int
ppm_addtocolorhash( colorhash_table cht, const pixel * const colorP, 
                    const int value );
/* Returns -1 on failure. */

colorhash_table
ppm_alloccolorhash( );

void
ppm_freecolorhash( colorhash_table cht );
