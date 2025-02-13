using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemoteMangerAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    PermissionId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Classification = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreatedById = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedById = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    PermissionId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Classification = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreatedById = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedById = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Username = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    PermissionId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Classification = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreatedById = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedById = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionsRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    PermissionsId = table.Column<string>(type: "character varying(32)", nullable: true),
                    RoleId = table.Column<string>(type: "character varying(32)", nullable: true),
                    CanRead = table.Column<bool>(type: "boolean", nullable: false),
                    CanWrite = table.Column<bool>(type: "boolean", nullable: false),
                    CanDelete = table.Column<bool>(type: "boolean", nullable: false),
                    PermissionId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Classification = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreatedById = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedById = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionsRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionsRoles_Permissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionsRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RDPAccounts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UserId = table.Column<string>(type: "character varying(32)", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Host = table.Column<string>(type: "text", nullable: true),
                    Account = table.Column<string>(type: "text", nullable: true),
                    Pwd = table.Column<string>(type: "text", nullable: true),
                    PermissionId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Classification = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreatedById = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedById = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RDPAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RDPAccounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserRadAccount",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UserId = table.Column<string>(type: "character varying(32)", nullable: true),
                    PermissionId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Classification = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreatedById = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedById = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRadAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRadAccount_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UserId = table.Column<string>(type: "character varying(32)", nullable: true),
                    RoleId = table.Column<string>(type: "character varying(32)", nullable: true),
                    PermissionId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Classification = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreatedById = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedById = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Classification", "CreatedById", "CreatedTime", "ModifiedById", "ModifiedTime", "Name", "PermissionId" },
                values: new object[] { "857B292E1609477B8C9D151300080A09", null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Default", "857B292E1609477B8C9D151300080A09" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Classification", "CreatedById", "CreatedTime", "ModifiedById", "ModifiedTime", "Name", "PermissionId" },
                values: new object[] { "b3ebc9a593a148d382bc7d5315de5821", null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Creater", "857B292E1609477B8C9D151300080A09" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Classification", "CreatedById", "CreatedTime", "Email", "ModifiedById", "ModifiedTime", "Password", "PermissionId", "Username" },
                values: new object[] { "a7643edb2960420d9e730e3812bf4da3", null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "11", "857B292E1609477B8C9D151300080A09", "11" });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionsRoles_PermissionsId",
                table: "PermissionsRoles",
                column: "PermissionsId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionsRoles_RoleId",
                table: "PermissionsRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RDPAccounts_UserId",
                table: "RDPAccounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRadAccount_UserId",
                table: "UserRadAccount",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PermissionsRoles");

            migrationBuilder.DropTable(
                name: "RDPAccounts");

            migrationBuilder.DropTable(
                name: "UserRadAccount");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
