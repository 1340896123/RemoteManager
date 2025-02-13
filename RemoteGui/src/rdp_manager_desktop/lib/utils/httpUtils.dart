import 'dart:convert';
import 'package:http/http.dart' as http;

class HttpService {
  static const String _defaultBaseURL = 'https://localhost'; // 默认服务器地址
  static const int _defaultPort = 7078; // 默认端口号

  String _baseURL = _defaultBaseURL;
  int _port = _defaultPort;
  String? _token; // 用于存储 Token

  // 设置全局服务器地址和端口号
  void setServer(String baseURL, int port) {
    _baseURL = baseURL;
    _port = port;
  }

  // 设置 Token
  void setToken(String token) {
    _token = token;
  }

  // 获取完整的请求 URL
  String _getFullUrl(String path) {
    return '$_baseURL:$_port$path';
  }

  // 处理响应头，自动设置 Token
  void _handleResponseHeaders(http.Response response) {
    if (response.headers.containsKey('token')) {
      final newToken = response.headers['token'];
      if (newToken != null) {
        setToken(newToken);
      }
    }
  }

  //final Map<String, String> headers = {};
  final Map<String, String> headers = {'Content-Type': 'application/json'};

  // 发送 GET 请求
  Future<http.Response> get(String path) async {
    final url = _getFullUrl(path);

    //final Map<String, String> headers =
    if (_token != null) {
      headers['Authorization'] = 'Bearer $_token';
    }

    final response = await http.get(Uri.parse(url), headers: headers);
    _handleResponseHeaders(response);
    return response;
  }

  // 发送 POST 请求
  Future<http.Response> post(String path, {body}) async {
    final url = _getFullUrl(path);

    if (_token != null) {
      headers['Authorization'] = 'Bearer $_token';
    }
    final response =
        await http.post(Uri.parse(url), headers: headers, body: body != null ? jsonEncode(body) : null);
    _handleResponseHeaders(response);
    return response;
  }

  // 发送 PUT 请求
  Future<http.Response> put(String path, {body}) async {
    final url = _getFullUrl(path);

    if (_token != null) {
      headers['Authorization'] = 'Bearer $_token';
    }
    final response =
        await http.put(Uri.parse(url), headers: headers, body: body != null ? jsonEncode(body) : null);
    _handleResponseHeaders(response);
    return response;
  }

  // 发送 DELETE 请求
  Future<http.Response> delete(String path) async {
    final url = _getFullUrl(path);

    if (_token != null) {
      headers['Authorization'] = 'Bearer $_token';
    }
    final response = await http.delete(Uri.parse(url), headers: headers);
    _handleResponseHeaders(response);
    return response;
  }
}
