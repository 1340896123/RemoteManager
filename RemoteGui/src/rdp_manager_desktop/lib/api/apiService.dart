import  'package:http/http.dart' as http;
import 'package:rdp_manager_desktop/utils/httpUtils.dart';

final httpService = HttpService();
// 登录
Future<http.Response> login(user) async {
  return httpService.post('/User/login', body: user);
}

// 注册
Future<http.Response> register(user) async {
  return httpService.post('/User/register', body: user);
}

// 创建 RDP 账户
Future<http.Response> createRdpAccount(request) async {
  return httpService.post('/User/rdp-accounts', body: request);
}

// 获取所有 RDP 账户
Future<http.Response> getRdpAccounts() async {
  return httpService.get('/User/rdp-accounts');
}

// 获取单个 RDP 账户
Future<http.Response> getRdpAccount(String id) async {
  return httpService.get('/User/rdp-accounts/$id');
}

// 更新 RDP 账户
Future<http.Response> updateRdpAccount(String id, request) async {
  return httpService.put('/User/rdp-accounts/$id', body: request);
}

// 删除 RDP 账户
Future<http.Response> deleteRdpAccount(String id) async {
  return httpService.delete('/User/rdp-accounts/$id');
}
