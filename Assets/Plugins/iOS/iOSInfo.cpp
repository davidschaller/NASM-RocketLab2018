#include <sys/sysctl.h>
#include <stdlib.h>

extern "C" char* GetDeviceModel()
{
	size_t size;
	// get string size by passing NULL
	sysctlbyname("hw.machine", NULL, &size, NULL, 0);
	
	char *machine = (char *)malloc(size + 1);
	
	sysctlbyname("hw.machine", machine, &size, NULL, 0);
	machine[size] = 0;
	
	return machine;
}