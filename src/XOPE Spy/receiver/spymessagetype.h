#pragma once

enum class SpyMessageType
{
	INVALID_MESSAGE,
	PING,
	PONG,
	ERROR_MESSAGE,
	INJECT_SEND,
	INJECT_RECV,
	CLOSE_SOCKET,	// Imitates a socket closing via recv returning 0 or send error being WSAENOTCONN
	IS_SOCKET_WRITABLE,
	REQUEST_SOCKET_INFO,
	TOGGLE_HTTP_TUNNELING,
	TOGGLE_INTERCEPTOR,
	STOP_HTTP_TUNNELING_SOCKET,
	ADD_PACKET_FITLER,
	MODIFY_PACKET_FILTER,
	TOGGLE_ACTIVATE_FILTER,
	DELETE_PACKET_FILTER,
	JOB_RESPONSE_SUCCESS,
	JOB_RESPONSE_ERROR,
	JOB_RESPONSE_DEFAULT, // This happens when the Job does not receive a response (e.g. UI has closed)
	SHUTDOWN_RECV_THREAD
};