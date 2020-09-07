CFLAGS = -O3 -Wall

.PHONY: all clean

all: shared

shared: Nuklear.c 
	$(CC) -fPIC -O3 -c Nuklear.c -I../nuklear_c
	$(CC) -shared -Wl,--export-dynamic Nuklear.o -o ../binaries/libnuklear.so
	rm -rf *.o

clean:
	rm -rf *.o *.so
