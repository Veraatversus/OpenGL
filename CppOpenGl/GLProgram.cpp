#include <fstream>
#include <sstream>

#include "GLProgram.h"


GLProgram::GLProgram(const GLchar** vertexShaderTexts, GLsizei vsCount, const GLchar** framentShaderTexts, GLsizei fsCount) :
	glVertexShader(0),
	glFragmentShader(0),
	glProgram(0)	
{
	glVertexShader = glCreateShader(GL_VERTEX_SHADER); checkAndThrow();
	glShaderSource(glVertexShader, vsCount, vertexShaderTexts, NULL); checkAndThrow();
	glCompileShader(glVertexShader); checkAndThrowShader(glVertexShader);
	
	glFragmentShader = glCreateShader(GL_FRAGMENT_SHADER); checkAndThrow();
	glShaderSource(glFragmentShader, fsCount, framentShaderTexts, NULL); checkAndThrow();
	glCompileShader(glFragmentShader); checkAndThrowShader(glFragmentShader);

	glProgram = glCreateProgram(); checkAndThrow();
	glAttachShader(glProgram, glVertexShader); checkAndThrow();
	glAttachShader(glProgram, glFragmentShader); checkAndThrow();
	glLinkProgram(glProgram); checkAndThrowProgram(glProgram);
}


GLProgram GLProgram::createFromFile(const std::string& vs, const std::string& fs) {
	return createFromFiles(std::vector<std::string>{vs},std::vector<std::string>{fs});
}

GLProgram GLProgram::createFromString(const std::string& vs, const std::string& fs) {
	return createFromStrings(std::vector<std::string>{vs},std::vector<std::string>{fs});
}

GLProgram GLProgram::createFromFiles(const std::vector<std::string>& vs, const std::vector<std::string>& fs) {
	std::vector<std::string> vsText{};
	std::vector<std::string> fsText{};
	
	for (std::string s : vs) {
		vsText.push_back(loadFile(s));
	}
	for (std::string s : fs) {
		fsText.push_back(loadFile(s));
	}

	return createFromStrings(vsText,fsText);
}

GLProgram GLProgram::createFromStrings(const std::vector<std::string>& vs, const std::vector<std::string>& fs) {
	std::vector<const GLchar*> vertexShaderTexts(vs.size());
	std::vector<const GLchar*> framentShaderTexts(fs.size());
		
	for (size_t i = 0;i<vs.size();++i) {
		vertexShaderTexts[i] = vs[i].c_str();
	}
	for (size_t i = 0;i<fs.size();++i) {
		framentShaderTexts[i] = fs[i].c_str();
	}
	
	return {vertexShaderTexts.data(), GLsizei(vs.size()), framentShaderTexts.data(), GLsizei(fs.size())};
}

std::string GLProgram::loadFile(const std::string& filename) {
	std::ifstream shaderFile(filename);
	if (!shaderFile) {
		throw ProgramException(std::string("Unable to open file ") + filename);
	}
	std::string str{};
	std::string all{};
	while (std::getline(shaderFile,str)) {
		all += str + "\n";
	}
	return all;
}

void GLProgram::checkAndThrow() {
	GLenum e = glGetError();
	if (e != GL_NO_ERROR) {
		std::stringstream s;
		s << "An openGL error occured:" << e;
		throw ProgramException{s.str()};
	}
}

GLint GLProgram::getUniformLocation(const std::string& name) const {
	return glGetUniformLocation(glProgram,name.c_str());
}

GLint GLProgram::getAttribLocation(const std::string& name) const {
	return glGetAttribLocation(glProgram,name.c_str());
}

void GLProgram::enable() const {
	glUseProgram(glProgram);	
}

void GLProgram::setUniform(GLint id, float value) const {
	glUniform1f(id, value);	
}

void GLProgram::setUniform(GLint id, const Vec3& value) const {
	glUniform3fv(id, 1, value);	
}

void GLProgram::setUniform(GLint id, const Mat4& value, bool transpose) const {
	glUniformMatrix4fv(id, 1, transpose, value);
}

void GLProgram::checkAndThrowShader(GLuint shader) {
	GLint success[1] = { GL_TRUE };
	glGetShaderiv(shader, GL_COMPILE_STATUS, success);
	if(success[0] == GL_FALSE) {
		GLint log_length{0};
		glGetShaderiv(shader, GL_INFO_LOG_LENGTH, &log_length);
		log_length = std::min(static_cast<GLint>(4096), log_length);
		std::vector<GLchar> log(log_length);
		glGetShaderInfoLog(shader, static_cast<GLsizei>(log.size()), NULL, log.data());
		std::string str{log.data()};
		throw ProgramException{str};
	}
}

void GLProgram::checkAndThrowProgram(GLuint program) {
	GLint linked{GL_TRUE};
	glGetProgramiv(program, GL_LINK_STATUS, &linked);
	if(linked != GL_TRUE) {
		GLint log_length{0};
		glGetProgramiv(program, GL_INFO_LOG_LENGTH, &log_length);
		log_length = std::min(static_cast<GLint>(4096), log_length);
		std::vector<GLchar> log(log_length);
		glGetProgramInfoLog(program, static_cast<GLsizei>(log.size()), NULL, log.data());
		std::string str{log.data()};
		throw ProgramException{str};
	}		
}