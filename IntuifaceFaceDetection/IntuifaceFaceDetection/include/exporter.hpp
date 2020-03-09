#pragma once


#include <memory>
#include <string>
#include <list>

#include <vector>
#include <map>
#include <opencv2/opencv.hpp>

#include "face.hpp"

class Exporter
{
public:
	using Ptr = std::shared_ptr<Exporter>;

	Exporter();

	void exportFaces(std::list<Face::Ptr> faces, size_t width, size_t height);
	void sendMessage(std::string msg);
	void poll();

	~Exporter();
};

