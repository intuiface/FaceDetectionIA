


#include "listener.hpp"
#include "shared_state.hpp"

#include <boost/asio/signal_set.hpp>
#include <boost/smart_ptr.hpp>


#include "exporter.hpp"
#include <iostream>

#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/json_parser.hpp>


using boost::property_tree::ptree;
using boost::property_tree::write_json;

namespace beast = boost::beast;         // from <boost/beast.hpp>
namespace http = beast::http;           // from <boost/beast/http.hpp>
namespace websocket = beast::websocket; // from <boost/beast/websocket.hpp>
namespace net = boost::asio;            // from <boost/asio.hpp>
using tcp = boost::asio::ip::tcp;       // from <boost/asio/ip/tcp.hpp>


/******* Web Sockets Classes *******/
boost::shared_ptr<shared_state> m_refSharedState;

// The io_context is required for all I/O
net::io_context ioc;

Exporter::Exporter()
{
	//start websocket server
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

void Exporter::poll()
{
	ioc.poll();
}


void Exporter::exportFaces(std::list<Face::Ptr> faces, size_t width, size_t height)
{
	//SME process faces list and generate JSON structure. 
	ptree root;
	root.put("Count", faces.size());

	// Add a list
	ptree viewers;
	for (auto &entry : faces)
	{
		// Create an unnamed node containing the value
		ptree viewer;
		viewer.put("id", entry->getId());
		
		//Add gender
		if (entry->isMale())
			viewer.put("gender", "male");
		else
			viewer.put("gender", "female");

		//add precise score to give more options on the IA side
		viewer.put("maleScore", entry->getMaleScore());
		viewer.put("femaleScore", entry->getFemaleScore());

		//Add age
		viewer.put("age", entry->getAge());

		//Build emotions list & main emotion
		for (auto &emotionPair: entry->getEmotions())
		{
			viewer.put("emotions."+emotionPair.first, emotionPair.second);
		}
		viewer.put("mainEmotion.emotion", entry->getMainEmotion().first);
		viewer.put("mainEmotion.confidence", entry->getMainEmotion().second);
		
		//Add face location using normalized coords
		viewer.put("location.x", (float)(entry->_location.x) / width);
		viewer.put("location.y", (float)(entry->_location.y) / height);
		viewer.put("location.width", (float)(entry->_location.width) / width);
		viewer.put("location.height", (float)(entry->_location.height) / height);

		// Add this node to the list.
		viewers.push_back(std::make_pair("", viewer));
	}
	root.add_child("viewers", viewers);

	//convert JSON object in string
	std::ostringstream buf;
	write_json(buf, root, false);
	std::string json = buf.str();

	//send structure through web socket
	sendMessage(json);

	//poll websocket
	ioc.poll();
}


void Exporter::sendMessage(std::string msg)
{	
	m_refSharedState->send(msg);	
}

Exporter::~Exporter()
{
	//TODO SME
}

