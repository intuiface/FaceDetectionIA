// ****************************************************************************
// <copyright file="exporter.hpp" company="Intuilab SAS">
// INTUILAB SAS
//_____________________
// [2002] - [2020] Intuilab SAS
// All Rights Reserved.
// NOTICE: All information contained herein is, and remains
// the property of Intuilab SAS.
// </copyright>
// ****************************************************************************

#pragma once

#include <memory>
#include <string>
#include <list>

#include "face.hpp"

/**
 * @brief class to export a given face list to JSON format.
 */
class Exporter
{
public:

	// ************************************************************************
	// FIELDS
	// ************************************************************************

	using Ptr = std::shared_ptr<Exporter>;


	// ************************************************************************
	// LIFE CYCLE
	// ************************************************************************

	Exporter();
	~Exporter();


	// ************************************************************************
	// OPERATIONS
	// ************************************************************************

	/**
	 * @brief export given face list to JSON format.
	 * @param faces: faces to export
	 * @param width: ???
	 * @param height: ???
	 */
	void exportFaces(std::list<Face::Ptr> faces, size_t width, size_t height);

	/**
	 * @brief send given message on websocket.
	 * @param msg: message to send
	 */
	void sendMessage(std::string msg);

	/**
	 * @brief poll...
	 */
	void poll();
};
