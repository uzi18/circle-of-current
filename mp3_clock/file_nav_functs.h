unsigned char open_next(DIR * dh, MP3File * mf, char * p)
{
	unsigned char looped = 0;
	unsigned char err;
	if(dh->index == 0)
	{
		// reopen directory if at beginning or if not open
		err = f_opendir(dh, p);
		if(err != 0) return err;
		looped = 1;
	}

	LCDSetPos(1, 1);
	fprintf_P(&LCDstdout, PSTR("Next Song:\n"));
	LCDClear(2);

	while(1)
	{
		FILINFO fno;
		f_readdir(dh, &fno); // open next file
		if(fno.fname[0] != 0) // file exists
		{
			if(MP3Open(&fno, mf, p) == 0) // if open
			{
				// print title and return successful

				fprintf_P(&LCDstdout, PSTR("%s\n"), mf->title);
				song_title_timer = 0;
				
				return 0;
			}
		}
		else
		{
			err = f_opendir(dh, p); // reopen
			if(err != 0) return err;
			if(looped != 0)
			{
				// already tried all files, return no file found
				return 255;
			}
			looped = 1; // looped all files
		}
	}
}

unsigned char shuffle(DIR * dh, MP3File * mf, char * p, unsigned char j)
{
	// shuffle just goes forward a random number of files and then open it

	LCDSetPos(1, 1);
	fprintf_P(&LCDstdout, PSTR("Shuffling\n"));
	LCDClear(2);

	srand(TCNT2);
	unsigned long t = rand() % j;
	for(unsigned char i = 0; i < t; i++)
	{
		FILINFO fno;
		f_readdir(dh, &fno);
		if(fno.fname[0] == 0)
		{
			f_opendir(dh, p);
		}
	}
	return open_next(dh, mf, p);
}

unsigned char open_prev(DIR * dh, MP3File * mf, char * p)
{
	// this function is the same as the open_next function
	// except this function remembers two files, if the current file matches
	// then the previous file is returned

	DIR tdir;
	unsigned char err = f_opendir(&tdir, p);
	if(err != 0) return err;

	LCDSetPos(1, 1);
	fprintf_P(&LCDstdout, PSTR("Previous Song:\n"));
	LCDClear(2);

	DIR ldir;
	MP3File tmf;
	MP3File lmf;

	unsigned char firstflag = 0;

	while(1)
	{
		FILINFO fno;
		f_readdir(&tdir, &fno);
		if(fno.fname[0] == 0)
		{
			break;
		}
		else
		{
			if(MP3Open(&fno, &tmf, p) == 0)
			{
				if(strcmp(mf->fn.n, tmf.fn.n) == 0)
				{
					if(firstflag != 0)
					{
						memcpy(dh, &ldir, sizeof(DIR));
						memcpy(mf, &lmf, sizeof(MP3File));
						fprintf_P(&LCDstdout, PSTR("%s\n"), mf->title);
						song_title_timer = 0;

						return 0;
					}
					else
					{
						firstflag = 2;
					}
				}
				else
				{
					memcpy(&ldir, &tdir, sizeof(DIR));
					memcpy(&lmf, &tmf, sizeof(MP3File));
					if(firstflag == 0)
					{
						firstflag = 1;
					}
				}
			}
		}
	}

	if(firstflag != 0)
	{
		memcpy(dh, &ldir, sizeof(DIR));
		memcpy(mf, &lmf, sizeof(MP3File));
		fprintf_P(&LCDstdout, PSTR("%s\n"), mf->title);
		song_title_timer = 0;

		return 0;
	}
	else
	{
		return 255;
	}
}
