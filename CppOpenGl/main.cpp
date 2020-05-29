#include <iostream>
#include <vector>


#include "GLEnv.h"
#include "GLProgram.h"
#include "GLBuffer.h"

#include "Mat4.h"
#include "Tesselation.h"



static void keyCallback(GLFWwindow* window, int key, int scancode, int action, int mods) {  
    if (key == GLFW_KEY_ESCAPE && action == GLFW_PRESS)
        glfwSetWindowShouldClose(window, GL_TRUE);
}

int main(int agrc, char ** argv) {    
    const GLEnv gl{640,480,4,"Interactive Late Night Coding Teil 2"};    
    gl.setKeyCallback(keyCallback);


    Tesselation sphere{Tesselation::genSphere({0,0,0},1,100,100)};

    GLBuffer vbPos{GL_ARRAY_BUFFER};
    vbPos.setData(sphere.vertices,3);

    GLBuffer vbNormal{GL_ARRAY_BUFFER};
    vbNormal.setData(sphere.normals,3);
    
    GLBuffer ib{GL_ELEMENT_ARRAY_BUFFER};
    ib.setData(sphere.indices);  

    GLProgram program{GLProgram::createFromFile("vertexShader.glsl", "fragmentShader.glsl")};

    GLint mvpLocation = program.getUniformLocation("MVP");
    GLint mitLocation = program.getUniformLocation("Mit");
    GLint mLocation = program.getUniformLocation("M");
    
    GLint lightPosLocation = program.getUniformLocation("lightPos");
    GLint posLocation = program.getAttribLocation("vPos");
    GLint normLocation = program.getAttribLocation("vNormal");
    
    vbPos.connectVertexAttrib(posLocation, 3);
    vbNormal.connectVertexAttrib(normLocation, 3);

    
    glEnable(GL_DEPTH_TEST);
    glDepthFunc(GL_LESS);    
    glClearDepth(1.0f);
    glClearColor(0.0f,0.0f,1.0f,0.0f);
    
    glEnable(GL_CULL_FACE);
    glCullFace(GL_BACK);
    
    do {
        const Dimensions dim{gl.getFramebufferSize()};
        
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
        
        glViewport(0, 0, dim.width, dim.height);

        Mat4 p{Mat4::perspective(90, dim.aspect(), 0.0001f, 1000.0f)};
        Mat4 m{Mat4::rotationY(glfwGetTime()*33)*Mat4::rotationZ(glfwGetTime()*20)};
        Mat4 v{Mat4::lookAt({0,0,2},{0,0,0},{0,1,0})};
        Mat4 mvp{m*v*p};
        
        program.enable();
        program.setUniform(mvpLocation, mvp);
        program.setUniform(mitLocation, Mat4::inverse(m), true);
        program.setUniform(mLocation,m);
        program.setUniform(lightPosLocation, {0,2,2});
        
        glDrawElements(GL_TRIANGLES, sphere.indices.size(), GL_UNSIGNED_INT, (void*)0);
        
        gl.endOfFrame();
    } while (!gl.shouldClose()); 
    
    
    return EXIT_SUCCESS;
}  
