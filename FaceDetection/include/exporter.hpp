// ****************************************************************************
// <copyright file="export.hpp" company="IntuiLab">
// INTUILAB CONFIDENTIAL
//_____________________
// [2002] - [2020] IntuiLab
// All Rights Reserved.
// NOTICE: All information contained herein is, and remains
// the property of IntuiLab. The intellectual and technical
// concepts contained herein are proprietary to IntuiLab
// and may be covered by U.S. and other country Patents, patents
// in process, and are protected by trade secret or copyright law.
// Dissemination of this information or reproduction of this
// material is strictly forbidden unless prior written permission
// is obtained from IntuiLab.
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
