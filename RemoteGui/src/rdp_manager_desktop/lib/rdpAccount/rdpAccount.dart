import 'dart:convert';
import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:process_run/shell.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:rdp_manager_desktop/api/apiService.dart' as api;

class RDPAccountListPage extends StatefulWidget {
  const RDPAccountListPage({super.key});

  @override
  _RDPAccountListPageState createState() => _RDPAccountListPageState();
}

class _RDPAccountListPageState extends State<RDPAccountListPage> {
  List<RDPAccount> _rdpAccounts = [];
  final TextEditingController _nameController = TextEditingController();
  final TextEditingController _hostController = TextEditingController();
  final TextEditingController _accountController = TextEditingController();
  final TextEditingController _pwdController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _loadRDPAccounts();
  }

  Future<void> _loadRDPAccounts() async {
    try {
      final response = await api.getRdpAccounts();
      if (response.statusCode == 200) {
        // 假设返回的 RDP 账户列表在响应体中
        final accounts = jsonDecode(response.body) as List;
        setState(() {
          _rdpAccounts =
              accounts.map((account) => RDPAccount.fromJson(account)).toList();
        });
      } else {
        // 如果请求失败，显示错误信息
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
              content: Text('Failed to load RDP accounts: ${response.body}')),
        );
      }
    } catch (e) {
      // 如果发生异常，显示错误信息
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Error loading RDP accounts: $e')),
      );
    }
  }

  Future<void> _saveRDPAccounts() async {
    final prefs = await SharedPreferences.getInstance();
    final accounts = _rdpAccounts.map((account) {
      return '${account.name},${account.host},${account.account},${account.pwd}';
    }).toList();
    await prefs.setStringList('rdp_accounts', accounts);
  }

// 新增 RDP 账户
  Future<void> _addRDPAccount() async {
    final newAccount = {
      'name': _nameController.text,
      'host': _hostController.text,
      'account': _accountController.text,
      'pwd': _pwdController.text,
    };

    final response = await api.createRdpAccount(newAccount);
    if (response.statusCode == 200) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('RDP account added successfully')),
      );
      _loadRDPAccounts(); // 刷新列表
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Failed to add RDP account: ${response.body}')),
      );
    }

    _nameController.clear();
    _hostController.clear();
    _accountController.clear();
    _pwdController.clear();
  }

  Future<void> _deleteRDPAccount(String id) async {
    try {
      final response = await api.deleteRdpAccount(id);
      if (response.statusCode == 200) {
        // 删除成功，从列表中移除对应的 RDP 账户
        setState(() {
          _rdpAccounts.removeWhere((account) => account.id == id);
        });
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('RDP account deleted successfully')),
        );
      } else {
        // 删除失败，显示错误信息
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
              content: Text('Failed to delete RDP account: ${response.body}')),
        );
      }
    } catch (e) {
      // 如果发生异常，显示错误信息
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Error deleting RDP account: $e')),
      );
    }
  }

  void _editRDPAccount(RDPAccount account) {
    _nameController.text = account.name;
    _hostController.text = account.host;
    _accountController.text = account.account;
    _pwdController.text = account.pwd;

    showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: Text('Edit RDP Account'),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                controller: _nameController,
                decoration: InputDecoration(labelText: 'Name'),
              ),
              TextField(
                controller: _hostController,
                decoration: InputDecoration(labelText: 'Host'),
              ),
              TextField(
                controller: _accountController,
                decoration: InputDecoration(labelText: 'Account'),
              ),
              TextField(
                controller: _pwdController,
                decoration: InputDecoration(labelText: 'Password'),
                obscureText: true,
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: () {
                Navigator.pop(context);
              },
              child: Text('Cancel'),
            ),
            TextButton(
              onPressed: () async {
                final updatedAccount = {
                  'name': _nameController.text,
                  'host': _hostController.text,
                  'account': _accountController.text,
                  'pwd': _pwdController.text,
                };

                final response =
                    await api.updateRdpAccount(account.id, updatedAccount);
                if (response.statusCode == 200) {
                  ScaffoldMessenger.of(context).showSnackBar(
                    SnackBar(content: Text('RDP account updated successfully')),
                  );
                  _loadRDPAccounts(); // 刷新列表
                } else {
                  ScaffoldMessenger.of(context).showSnackBar(
                    SnackBar(
                        content: Text(
                            'Failed to update RDP account: ${response.body}')),
                  );
                }
                Navigator.pop(context);
              },
              child: Text('Save'),
            ),
          ],
        );
      },
    );
  }

  Future<String> convertToSecureString(String password) async {
    final command = '''
powershell -c "\$secureString = '$password' | ConvertTo-SecureString -AsPlainText -Force; \$secureString | ConvertFrom-SecureString"
''';
    final shell = Shell();
    final results = await shell.run(command);

    // 检查每个命令的执行结果
    for (var result in results) {
      if (result.exitCode != 0) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Command failed: ${result.stderr}')),
        );
      }
      return result.outText;
    }
    return "";
  }

  void _connectRDPAccount(RDPAccount account) async {
    final ddlinw = await convertToSecureString(account.pwd);
    final rdpContent = '''
full address:s:${account.host}
username:s:${account.account}
password 51:b:$ddlinw
''';

    final rdpFile = File('temp.rdp');
    await rdpFile.writeAsString(rdpContent);

    final command = 'cmd /c mstsc ${rdpFile.path}';
    await Shell().run(command);

    // 删除临时文件
    await rdpFile.delete();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('RDP Connections',
            style: TextStyle(fontWeight: FontWeight.bold)),
        elevation: 4,
      ),
      body: Container(
        padding: const EdgeInsets.all(8),
        decoration: BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topCenter,
            end: Alignment.bottomCenter,
            colors: [
              Theme.of(context).colorScheme.primaryContainer,
              Theme.of(context).colorScheme.secondaryContainer
            ],
          ),
        ),
        child: Column(
          children: [
            // 列表头标题（对齐关键：使用与列表项完全相同的列结构）
            Card(
              elevation: 2,
              shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12)),
              child: Padding(
                padding:
                    const EdgeInsets.symmetric(vertical: 12, horizontal: 16),
                child: Row(
                  children: [
                    // 对应列表项的 leading 图标占位
                    const SizedBox(width: 72), // 关键修改点 1：添加 leading 占位
                    // 列内容
                    Expanded(
                        flex: 2,
                        child: Text('Name', style: _headerTextStyle(context))),
                    Expanded(
                        flex: 3,
                        child: Text('Host', style: _headerTextStyle(context))),
                    Expanded(
                        flex: 2,
                        child:
                            Text('Account', style: _headerTextStyle(context))),
                    Expanded(
                        flex: 2,
                        child:
                            Text('Password', style: _headerTextStyle(context))),
                    // 对应列表项的 trailing 按钮占位
                    const SizedBox(width: 120), // 关键修改点 2：固定 trailing 宽度
                  ],
                ),
              ),
            ),
            Expanded(
              child: ListView.separated(
                itemCount: _rdpAccounts.length,
                separatorBuilder: (context, index) => const SizedBox(height: 8),
                itemBuilder: (context, index) {
                  final account = _rdpAccounts[index];
                  return _buildListItem(context, account);
                },
              ),
            ),
          ],
        ),
      ),
      floatingActionButton: Stack(
        children: [
          Positioned(
            bottom: 0,
            right: 0,
            child: FloatingActionButton(
              heroTag: 'RDPADD', // 设置唯一的标签
              onPressed: () {
                showDialog(
                  context: context,
                  builder: (context) {
                    return AlertDialog(
                      title: Text('Add RDP Account'),
                      content: Column(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          TextField(
                            controller: _nameController,
                            decoration: InputDecoration(labelText: 'Name'),
                          ),
                          TextField(
                            controller: _hostController,
                            decoration: InputDecoration(labelText: 'Host'),
                          ),
                          TextField(
                            controller: _accountController,
                            decoration: InputDecoration(labelText: 'Account'),
                          ),
                          TextField(
                            controller: _pwdController,
                            decoration: InputDecoration(labelText: 'Password'),
                            obscureText: true,
                          ),
                        ],
                      ),
                      actions: [
                        TextButton(
                          onPressed: () {
                            Navigator.pop(context);
                          },
                          child: Text('Cancel'),
                        ),
                        TextButton(
                          onPressed: _addRDPAccount,
                          child: Text('Add'),
                        ),
                      ],
                    );
                  },
                );
              },
              child: Icon(Icons.add),
            ),
          ),
          Positioned(
            bottom: 0,
            right: 70, // 调整第二个按钮的位置，避免重叠
            child: FloatingActionButton(
              heroTag: 'RDPReload', // 设置唯一的标签
              onPressed: () {
                _loadRDPAccounts();
                // 这里是新按钮的功能
                // 例如：显示一个提示或执行其他操作
                ScaffoldMessenger.of(context).showSnackBar(
                  SnackBar(content: Text('刷新成功')),
                );
              },
              child: Icon(Icons.refresh), // 使用不同的图标
            ),
          ),
        ],
      ),
    );
  }

  // 列表头样式
  TextStyle _headerTextStyle(BuildContext context) {
    return TextStyle(
        fontSize: 14,
        fontWeight: FontWeight.w600,
        color: Theme.of(context).colorScheme.primary,
        letterSpacing: 0.5);
  }

// 修改后的列表项组件（关键修改点 3：使用与标题相同的列结构）
  Widget _buildListItem(BuildContext context, RDPAccount account) {
    return Card(
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.symmetric(
            vertical: 8, horizontal: 16), // 增加水平 padding
        child: Row(
          children: [
            // Leading 图标（固定宽度）
            Container(
              width: 40, // 固定宽度（72 - padding）
              padding: const EdgeInsets.all(10),
              decoration: BoxDecoration(
                color: Theme.of(context).colorScheme.primary,
                shape: BoxShape.circle,
              ),
              child: Center(
                // 使用 Center 将图标居中
                child: Icon(
                  Icons.desktop_windows,
                  color: Theme.of(context).colorScheme.onPrimary,
                  size: 20,
                ),
              ),
            ),
            const SizedBox(width: 32), // 关键修改点 4：总占位宽度 = 40 + 32 = 72
            // 内容列
            Expanded(
                flex: 2,
                child: Row(children: [
                  Text(account.name,
                      style: TextStyle(
                          fontSize: 12,
                          color: Theme.of(context).colorScheme.onSurface)),
                  IconButton(
                    icon: Icon(Icons.copy, size: 20),
                    color: Theme.of(context).colorScheme.primary,
                    onPressed: () {
                      Clipboard.setData(ClipboardData(text: account.name));
                      ScaffoldMessenger.of(context).showSnackBar(
                        SnackBar(content: Text('Name copied to clipboard')),
                      );
                    },
                  ),
                ])),
            Expanded(
                flex: 2,
                child: Row(children: [
                  Text(account.host,
                      style: TextStyle(
                          fontSize: 12,
                          color: Theme.of(context).colorScheme.onSurface)),
                  IconButton(
                    icon: Icon(Icons.copy, size: 20),
                    color: Theme.of(context).colorScheme.primary,
                    onPressed: () {
                      Clipboard.setData(ClipboardData(text: account.host));
                      ScaffoldMessenger.of(context).showSnackBar(
                        SnackBar(content: Text('Host copied to clipboard')),
                      );
                    },
                  ),
                ])),
            Expanded(
                flex: 2,
                child: Row(children: [
                  Text(account.account,
                      style: TextStyle(
                          fontSize: 12,
                          color: Theme.of(context).colorScheme.onSurface)),
                  IconButton(
                    icon: Icon(Icons.copy, size: 20),
                    color: Theme.of(context).colorScheme.primary,
                    onPressed: () {
                      Clipboard.setData(ClipboardData(text: account.account));
                      ScaffoldMessenger.of(context).showSnackBar(
                        SnackBar(content: Text('Account copied to clipboard')),
                      );
                    },
                  ),
                ])),
            Expanded(
              flex: 2,
              child: Row(
                children: [
                  Text("*****", // 显示隐藏的密码
                      style: TextStyle(
                          fontSize: 12,
                          color: Theme.of(context).colorScheme.onSurface)),
                  IconButton(
                    icon: Icon(Icons.copy, size: 20),
                    color: Theme.of(context).colorScheme.primary,
                    onPressed: () {
                      Clipboard.setData(ClipboardData(text: account.pwd));
                      ScaffoldMessenger.of(context).showSnackBar(
                        SnackBar(content: Text('Password copied to clipboard')),
                      );
                    },
                  ),
                ],
              ),
            ),
            // 操作按钮（固定宽度）
            SizedBox(
              width: 120,
              child: Row(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  IconButton(
                    icon: Icon(Icons.edit, size: 20),
                    color: Theme.of(context).colorScheme.primary,
                    onPressed: () => _editRDPAccount(account),
                  ),
                  IconButton(
                    icon: Icon(Icons.delete, size: 20),
                    color: Theme.of(context).colorScheme.error,
                    onPressed: () => _deleteRDPAccount(account.id),
                  ),
                  IconButton(
                    icon: Icon(Icons.connect_without_contact),
                    onPressed: () => _connectRDPAccount(account),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class RDPAccount {
  final String id;
  String name;
  String host;
  String account;
  String pwd;

  RDPAccount({
    required this.id,
    required this.name,
    required this.host,
    required this.account,
    required this.pwd,
  });

  factory RDPAccount.fromJson(Map<String, dynamic> json) {
    return RDPAccount(
      id: json['id'],
      name: json['name'],
      host: json['host'],
      account: json['account'],
      pwd: json['pwd'],
    );
  }
}
