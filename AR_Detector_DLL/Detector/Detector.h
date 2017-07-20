// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the DETECTOR_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// DETECTOR_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef DETECTOR_EXPORTS
#define DETECTOR_API __declspec(dllexport)
#else
#define DETECTOR_API __declspec(dllimport)
#endif

// This class is exported from the Detector.dll
class DETECTOR_API CDetector {
public:
	CDetector(void);
	// TODO: add your methods here.
};

extern DETECTOR_API int nDetector;

DETECTOR_API int fnDetector(void);
