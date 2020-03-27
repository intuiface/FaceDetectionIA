// ****************************************************************************
// <copyright file="export.cpp" company="IntuiLab">
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

#include <iostream>
#include <boost/asio/signal_set.hpp>
#include <boost/smart_ptr.hpp>
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/json_parser.hpp>

#include "listener.hpp"
#include "shared_state.hpp"

#include "exporter.hpp"

using boost::property_tree::ptree;
using boost::property_tree::write_json;

namespace beast = boost::beast;         // from <boost/beast.hpp>
namespace http = beast::http;           // from <boost/beast/http.hpp>
namespace websocket = beast::websocket; // from <boost/beast/websocket.hpp>
namespace net = boost::asio;            // from <boost/asio.hpp>
using tcp = boost::asio::ip::tcp;       // from <boost/asio/ip/tcp.hpp>


// Web socket shared state
boost::shared_ptr<shared_state> m_refSharedState;

// The io_context is required for all I/O
net::io_context ioc;


// ************************************************************************
// LIFE CYCLE
// ************************************************************************

Exporter::Exporter()
{
	// Start websocket server
	auto address = net::ip::make_address("0.0.0.0");
	auto port = static_cast<unsigned short>(std::atoi("2975"));
	auto doc_root = ".";
	auto const threads = std::max<int>(1, std::atoi("5"));

	m_refSharedState = boost::make_shared<shared_state>(doc_root);

	// Create and launch a listening port
	boost::make_shared<listener>(
		ioc,
		tcp::endpoint{ address, port },
		m_refSharedState)->run();
}

Exporter::~Exporter()
{

}


// ************************************************************************
// OPERATIONS
// ************************************************************************

void Exporter::exportFaces(std::list<Face::Ptr> faces, size_t width, size_t height)
{
	// Create a property tree and add Face Count property
	ptree root;
	root.put("Count", faces.size());

	float fInvWidth = 1.0 / (float)width;
	float fInvHeight = 1.0 / (float)height;

	// Add a list of faces informations
	ptree faceList;
	for (auto &entry: faces)
	{
		// Create a node containing all informations of a face
		ptree faceProperties;
		faceProperties.put("id", entry->getId());
		
		// Add gender
		faceProperties.put("gender", entry->isMale() ? "male" : "female");

		// Add age
		faceProperties.put("age", entry->getAge());

		// Add main emotion and score for each emotion
		faceProperties.put("mainEmotion.emotion", entry->getMainEmotion().first);
		faceProperties.put("mainEmotion.confidence", entry->getMainEmotion().second);
		for (auto &emotionPair: entry->getEmotions())
		{
			faceProperties.put("emotions." + emotionPair.first, emotionPair.second);
		}
		
		// Add face location using normalized coordinates
		faceProperties.put("location.x", (float)(entry->_location.x) * fInvWidth);
		faceProperties.put("location.y", (float)(entry->_location.y) * fInvHeight);
		faceProperties.put("location.width", (float)(entry->_location.width) * fInvWidth);
		faceProperties.put("location.height", (float)(entry->_location.height) * fInvHeight);

		// Add head position estimation
		auto headPose = entry->getHeadPose();
		faceProperties.put("headpose.pitch", headPose.angle_p);
		faceProperties.put("headpose.yaw", headPose.angle_y);
		faceProperties.put("headpose.roll", headPose.angle_r);

		// Add informations of this face to the face list
		faceList.push_back(std::make_pair("", faceProperties));
	}
	root.add_child("faces", faceList);

	// Convert JSON to string
	std::ostringstream buffer;
	write_json(buffer, root, false);
	std::string json = buffer.str();

	// Send structure through web socket
	sendMessage(json);

	// Poll websocket
	ioc.poll();
}

void Exporter::sendMessage(std::string msg)
{	
	m_refSharedState->send(msg);
}

void Exporter::poll()
{
	ioc.poll();
}
