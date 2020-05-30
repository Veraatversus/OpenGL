#pragma once

#include <vector>

#include "Vec3.h"
#include "Mat4.h"
#include "GLProgram.h"
#include "GLBuffer.h"
#include "GLTexture2D.h"

class Particle {
public:
	Particle(const Vec3& position, const Vec3& direction, const Vec3& acceleration, 
			 const Vec3& color, float opacity, uint32_t maxAge, 
			 const Vec3& minPos, const Vec3& maxPos, bool bounce);
	
	void update();
	void restart(const Vec3& position, const Vec3& direction, const Vec3& color, float opacity);
	bool isDead() const {return age >= maxAge;};
	void setBounce(bool bounce) {this->bounce = bounce;}
	void setAcceleration(const Vec3& acceleration) {this->acceleration = acceleration;}
	
	std::vector<float> getData() const;
	
private:
	Vec3 position;
	Vec3 direction;
	Vec3 acceleration;
	Vec3 color;
	float opacity;
	bool bounce;
	
	uint32_t maxAge;
	uint32_t age;
	
	Vec3 minPos;
	Vec3 maxPos;
};


class ParticleSystem {
public:
	ParticleSystem(uint32_t particleCount, const Vec3& center, float spreadRadius, const Vec3& acceleration, const Vec3& minPos, const Vec3& maxPos, uint32_t maxAge);

	void render(const Mat4& v, const Mat4& p) const;
	void update();
	
	void setBounce(bool bounce);
	void setAcceleration(const Vec3& acceleration);

private:
	Vec3 center;
	float spreadRadius;	
	std::vector<Particle> particles;
	
	GLProgram prog;
	GLint mvpLocation;
	GLint posLocation;
	GLint colLocation;
	GLint texLocation;
	
	GLTexture2D sprite;
	
	Vec3 computeCenter() const;
	Vec3 computeDirection() const;
	Vec3 computeColor() const;
	
};