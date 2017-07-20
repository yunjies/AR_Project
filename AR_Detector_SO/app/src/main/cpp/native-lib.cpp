#include <jni.h>
#include <string>

JNIEXPORT jcharArray JNICALL
Java_com_yunjies_ar_1detector_1socopy_MyNDK_GetAllMarkers(JNIEnv *env, jobject instance,
                                                          jcharArray data_, jint width,
                                                          jint height) {
    jchar *data = env->GetCharArrayElements(data_, NULL);

    // TODO

    env->ReleaseCharArrayElements(data_, data, 0);
}

extern "C"
JNIEXPORT jstring JNICALL
Java_com_yunjies_ar_1detector_1socopy_MainActivity_stringFromJNI(
        JNIEnv *env,
        jobject /* this */) {
    std::string hello = "Hello from C++";
    return env->NewStringUTF(hello.c_str());
}
