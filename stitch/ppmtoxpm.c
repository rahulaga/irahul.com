#include <stdio.h>
#include <ctype.h>
#include <string.h>
#include "ppm.h"
#include "ppmcmap.h"

/* Max number of colors allowed in ppm input. */
#define MAXCOLORS    256

/* Max number of rgb mnemonics allowed in rgb text file. */
#define MAX_RGBNAMES 1024

#define MAXPRINTABLE 92			/* number of printable ascii chars
					 * minus \ and " for string compat
					 * and ? to avoid ANSI trigraphs. */

static char *printable =
" .XoO+@#$%&*=-;:>,<1234567890qwertyuipasdfghjklzxcvbnmMNBVCZ\
ASDFGHJKLPIUYTREWQ!~^/()_`'][{}|";


#define max(a,b) ((a) > (b) ? (a) : (b))

void read_rgb_names();			/* forward reference */
void gen_cmap();			/* forward reference */

typedef struct {			/* rgb values and ascii names (from
					 * rgb text file) */
    int r, g, b;			/* rgb values, range of 0 -> 65535 */
    char *name;				/* color mnemonic of rgb value */
}      rgb_names;

typedef struct {			/* character-pixel mapping */
    char *cixel;			/* character string printed for
					 * pixel */
    char *rgbname;			/* ascii rgb color, either color
					 * mnemonic or #rgb value */
}      cixel_map;

pixel **pixels;



int ppmtoxpm(char* file1,char* file2){
    FILE *ifd,*ofd;
    register pixel *pP;
    int  rows, cols, ncolors, row, col, i;
    pixval maxval;			/* pixval == unsigned char or
					 * unsigned short */
    colorhash_table cht;
    colorhist_vector chv;

    /* Used for rgb value -> rgb mnemonic mapping */
    int map_rgb_names = 0;
    rgb_names rgbn[MAX_RGBNAMES];
    int rgbn_max;

    /* Used for rgb value -> character-pixel string mapping */
    cixel_map cmap[MAXCOLORS];
    int charspp;			/* chars per pixel */

    char out_name[100], rgb_fname[100], *cp;

    //ppm_init(&argc, argv);
    rgb_fname[0] = '\0';

    //sscanf(argv[0], "%s", out_name);
    strcpy(out_name,file2);
    
    ifd = pm_openr(file1);
    ofd = fopen(file2,"w");
    /*
     * "maxval" is the largest value that can be be found in the ppm file.
     * All pixel components are relative to this value. 
     */
    pixels = ppm_readppm(ifd, &cols, &rows, &maxval);
    pm_close(ifd);

    /* Figure out the colormap. */
    //pm_message("(Computing colormap...");
    chv = ppm_computecolorhist(pixels, cols, rows, MAXCOLORS, &ncolors);
    if (chv == (colorhist_vector) 0)
	pm_error(
	 "too many colors - try running the pixmap through 'ppmquant 256'",
		 0, 0, 0, 0, 0);
   // pm_message("...Done.  %d colors found.)\n", ncolors);

    /* Make a hash table for fast color lookup. */
    cht = ppm_colorhisttocolorhash(chv, ncolors);

    /*
     * If a rgb text file was specified, read in the rgb mnemonics. Does not
     * return if fatal error occurs. 
     */
    if (map_rgb_names)
	read_rgb_names(rgb_fname, rgbn, &rgbn_max);

    /* Now generate the character-pixel colormap table. */
    gen_cmap(chv, ncolors, maxval, map_rgb_names, rgbn, rgbn_max,
	     cmap, &charspp);

    /* Write out the XPM file. */

    fprintf(ofd,"/* XPM */\n");
    fprintf(ofd,"static char *%s[] = {\n", out_name);
    fprintf(ofd,"/* width height ncolors chars_per_pixel */\n");
    fprintf(ofd,"\"%d %d %d %d\",\n", cols, rows, ncolors, charspp);
    fprintf(ofd,"/* colors */\n");
    for (i = 0; i < ncolors; i++) {
	fprintf(ofd,"\"%s c %s\",\n", cmap[i].cixel, cmap[i].rgbname);
    }
    fprintf(ofd,"/* pixels */\n");
    for (row = 0; row < rows; row++) {
	fprintf(ofd,"\"");
	for (col = 0, pP = pixels[row]; col < cols; col++, pP++) {
	    fprintf(ofd,"%s", cmap[ppm_lookupcolor(cht, pP)].cixel);
	}
	fprintf(ofd,"\"%s\n", (row == (rows - 1) ? "" : ","));
    }
    fprintf(ofd,"};\n");
    fflush(ofd); 
    fclose(ofd);
    /* If the program failed, it previously aborted with nonzero completion
       code, via various function calls.
    */
    return 0;
}					/* main */

/*---------------------------------------------------------------------------*/
/* This routine reads a rgb text file.  It stores the rgb values (0->65535)
   and the rgb mnemonics (malloc'ed) into the "rgbn" array.  Returns the
   number of entries stored in "rgbn_max". */
void
read_rgb_names(rgb_fname, rgbn, rgbn_max)
    char *rgb_fname;
    rgb_names rgbn[MAX_RGBNAMES];
int *rgbn_max;

{
    FILE *rgbf;
    int i, items, red, green, blue;
    char line[512], name[512], *rgbname;

    /* Open the rgb text file.  Abort if error. */
    if ((rgbf = fopen(rgb_fname, "r")) == NULL)
	pm_error("error opening rgb text file \"%s\"", rgb_fname, 0, 0, 0, 0);

    /* Loop reading each line in the file. */
    for (i = 0; fgets(line, sizeof(line), rgbf); i++) {

	/* Quit if rgb text file is too large. */
	if (i == MAX_RGBNAMES) {
	    fprintf(stderr,
	    "Too many entries in rgb text file, truncated to %d entries.\n",
		    MAX_RGBNAMES);
	    fflush(stderr);
	    break;
	}
	/* Read the line.  Skip if bad. */
	items = sscanf(line, "%d %d %d %[^\n]\n", &red, &green, &blue, name);
	if (items != 4) {
	    fprintf(stderr, "rgb text file syntax error on line %d\n", i + 1);
	    fflush(stderr);
	    i--;
	    continue;
	}
	/* Make sure rgb values are within 0->255 range.  Skip if bad. */
	if (red < 0 || red > 0xFF ||
	    green < 0 || green > 0xFF ||
	    blue < 0 || blue > 0xFF) {
	    fprintf(stderr, "rgb value for \"%s\" out of range, ignoring it\n",
		    name);
	    fflush(stderr);
	    i--;
	    continue;
	}
	/* Allocate memory for ascii name.  Abort if error. */
	if (!(rgbname = (char *) malloc(strlen(name) + 1)))
	    pm_error("out of memory allocating rgb name", 0, 0, 0, 0, 0);
	    
#ifdef NAMESLOWCASE
	/* Copy string to ascii name and lowercase it. */
	for (n = name, m = rgbname; *n; n++)
	    *m++ = isupper(*n) ? tolower(*n) : *n;
	*m = '\0';
#else
	strcpy(rgbname, name);
#endif

	/* Save the rgb values and ascii name in the array. */
	rgbn[i].r = red << 8;
	rgbn[i].g = green << 8;
	rgbn[i].b = blue << 8;
	rgbn[i].name = rgbname;
    }

    /* Return the max number of rgb names. */
    *rgbn_max = i - 1;

    fclose(rgbf);

}					/* read_rgb_names */

/*---------------------------------------------------------------------------*/
/* Given a number and a base (MAXPRINTABLE), this routine
   prints the number into a malloc'ed string and returns it.  The length of
   the string is specified by "digits".  The ascii characters of the printed
   number range from printable[0] to printable[MAXPRINTABLE].  The string is
   printable[0] filled, (e.g. if printable[0]==0, printable[1]==1,
   MAXPRINTABLE==2, digits==5, i=3, routine would return the malloc'ed
   string "00011"). */
char *
gen_numstr(i, digits)
    int i, digits;
{
    char *str, *p;
    int d;

    /* Allocate memory for printed number.  Abort if error. */
    if (!(str = (char *) malloc(digits + 1)))
	pm_error("out of memory", 0, 0, 0, 0, 0);

    /* Generate characters starting with least significant digit. */
    p = str + digits;
    *p-- = '\0';			/* nul terminate string */
    while (p >= str) {
	d = i % MAXPRINTABLE;
	i /= MAXPRINTABLE;
	*p-- = printable[d];
    }

    return str;

}					/* gen_numstr */

/*---------------------------------------------------------------------------*/
/* This routine generates the character-pixel colormap table. */
void
gen_cmap(chv, ncolors, maxval, map_rgb_names, rgbn, rgbn_max,
	 cmap, charspp)
/* input: */
    colorhist_vector chv;		/* contains rgb values for colormap */
    int ncolors;			/* number of entries in colormap */
    pixval maxval;			/* largest color value, all rgb
					 * values relative to this, (pixval
					 * == unsigned short) */
    int map_rgb_names;			/* == 1 if mapping rgb values to rgb
					 * mnemonics */
    rgb_names rgbn[MAX_RGBNAMES];	/* rgb mnemonics from rgb text file */
int rgbn_max;				/* number of rgb mnemonics in table */

/* output: */
cixel_map cmap[MAXCOLORS];		/* pixel strings and ascii rgb
					 * colors */
int *charspp;				/* characters per pixel */

{
    int i, j, cpp, mval, red, green, blue, r, g, b, matched;
    char *str;

    /*
     * Figure out how many characters per pixel we'll be using.  Don't want
     * to be forced to link with libm.a, so using a division loop rather
     * than a log function. 
     */
    for (cpp = 0, j = ncolors; j; cpp++)
	j /= MAXPRINTABLE;
    *charspp = cpp;

    /*
     * Determine how many hex digits we'll be normalizing to if the rgb
     * value doesn't match a color mnemonic. 
     */
    mval = (int) maxval;
    if (mval <= 0x000F)
	mval = 0x000F;
    else if (mval <= 0x00FF)
	mval = 0x00FF;
    else if (mval <= 0x0FFF)
	mval = 0x0FFF;
    else
	mval = 0xFFFF;

    /*
     * Generate the character-pixel string and the rgb name for each
     * colormap entry. 
     */
    for (i = 0; i < ncolors; i++) {

	/*
	 * The character-pixel string is simply a printed number in base
	 * MAXPRINTABLE where the digits of the number range from
	 * printable[0] .. printable[MAXPRINTABLE-1] and the printed length
	 * of the number is "cpp". 
	 */
	cmap[i].cixel = gen_numstr(i, cpp);

	/* Fetch the rgb value of the current colormap entry. */
	red = PPM_GETR(chv[i].color);
	green = PPM_GETG(chv[i].color);
	blue = PPM_GETB(chv[i].color);

	/*
	 * If the ppm color components are not relative to 15, 255, 4095,
	 * 65535, normalize the color components here. 
	 */
	if (mval != (int) maxval) {
	    red = (red * mval) / (int) maxval;
	    green = (green * mval) / (int) maxval;
	    blue = (blue * mval) / (int) maxval;
	}

	/*
	 * If the "-rgb <rgbfile>" option was specified, attempt to map the
	 * rgb value to a color mnemonic. 
	 */
	if (map_rgb_names) {

	    /*
	     * The rgb values of the color mnemonics are normalized relative
	     * to 255 << 8, (i.e. 0xFF00).  [That's how the original MIT
	     * code did it, really should have been "v * 65535 / 255"
	     * instead of "v << 8", but have to use the same scheme here or
	     * else colors won't match...]  So, if our rgb values aren't
	     * already 16-bit values, need to shift left. 
	     */
	    if (mval == 0x000F) {
		r = red << 12;
		g = green << 12;
		b = blue << 12;
		/* Special case hack for "white". */
		if (0xF000 == r && r == g && g == b)
		    r = g = b = 0xFF00;
	    } else if (mval == 0x00FF) {
		r = red << 8;
		g = green << 8;
		b = blue << 8;
	    } else if (mval == 0x0FFF) {
		r = red << 4;
		g = green << 4;
		b = blue << 4;
	    } else {
		r = red;
		g = green;
		b = blue;
	    }

	    /*
	     * Just perform a dumb linear search over the rgb values of the
	     * color mnemonics.  One could speed things up by sorting the
	     * rgb values and using a binary search, or building a hash
	     * table, etc... 
	     */
	    for (matched = 0, j = 0; j <= rgbn_max; j++)
		if (r == rgbn[j].r && g == rgbn[j].g && b == rgbn[j].b) {

		    /* Matched.  Allocate string, copy mnemonic, and exit. */
		    if (!(str = (char *) malloc(strlen(rgbn[j].name) + 1)))
			pm_error("out of memory", 0, 0, 0, 0, 0);
		    strcpy(str, rgbn[j].name);
		    cmap[i].rgbname = str;
		    matched = 1;
		    break;
		}
	    if (matched)
		continue;
	}

	/*
	 * Either not mapping to color mnemonics, or didn't find a match.
	 * Generate an absolute #RGB value string instead. 
	 */
	if (!(str = (char *) malloc(mval == 0x000F ? 5 :
				    mval == 0x00FF ? 8 :
				    mval == 0x0FFF ? 11 :
				    14)))
	    pm_error("out of memory", 0, 0, 0, 0, 0);

	sprintf(str, mval == 0x000F ? "#%X%X%X" :
		mval == 0x00FF ? "#%02X%02X%02X" :
		mval == 0x0FFF ? "#%03X%03X%03X" :
		"#%04X%04X%04X", red, green, blue);
	cmap[i].rgbname = str;
    }

}					/* gen_cmap */
