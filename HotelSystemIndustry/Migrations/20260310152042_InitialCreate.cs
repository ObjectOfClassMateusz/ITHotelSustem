using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelSystemIndustry.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeProfile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Value = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventReservationStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Value = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventReservationStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Value = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HousekeepingSupplies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Unit = table.Column<int>(type: "integer", nullable: false),
                    QuantityInStock = table.Column<decimal>(type: "numeric", nullable: false),
                    MinimumRequiredQuantity = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HousekeepingSupplies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KitchenArticleTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Value = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenArticleTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KitchenOrderTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Value = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenOrderTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KitchenStorages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Location = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenStorages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Raport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Raport", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecreationFacilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    MaxCapacity = table.Column<long>(type: "bigint", nullable: false),
                    PricePerHour = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecreationFacilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaleItemTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Value = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleItemTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopMagazines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Location = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopMagazines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Location = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeShifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeShifts_EmployeeProfile_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipment_EquipmentTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "EquipmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NumRequiredStaff = table.Column<long>(type: "bigint", nullable: false),
                    NumGuests = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventReservations_EventReservationStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "EventReservationStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventReservations_EventTypes_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "EventTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Street = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    PostalCode = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    HotelId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => new { x.Street, x.City, x.PostalCode });
                    table.ForeignKey(
                        name: "FK_Address_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Phone",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    HotelId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Phone_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KitchenArticles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KitchenArticles_KitchenArticleTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "KitchenArticleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KitchenOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmissionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeliveryDestination = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KitchenOrders_KitchenOrderTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "KitchenOrderTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecreationBookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GuestId = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecreationBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecreationBookings_Guest_GuestId",
                        column: x => x.GuestId,
                        principalTable: "Guest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecreationBookings_RecreationFacilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "RecreationFacilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContainsAlcohol = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleItems_SaleItemTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "SaleItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ShopPointId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Purchases_ShopPoints_ShopPointId",
                        column: x => x.ShopPointId,
                        principalTable: "ShopPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventHalls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    NumMaxGuests = table.Column<long>(type: "bigint", nullable: false),
                    ReservationPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    EventReservationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventHalls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventHalls_EventReservations_EventReservationId",
                        column: x => x.EventReservationId,
                        principalTable: "EventReservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomNumber = table.Column<string>(type: "text", nullable: false),
                    Floor = table.Column<int>(type: "integer", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    BasePricePerNight = table.Column<decimal>(type: "numeric", nullable: false),
                    Renovation = table.Column<bool>(type: "boolean", nullable: false),
                    RoomType = table.Column<int>(type: "integer", nullable: false),
                    HotelId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventReservationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Room_EventReservations_EventReservationId",
                        column: x => x.EventReservationId,
                        principalTable: "EventReservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Room_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KitchenArticleInstances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StorageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Count = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenArticleInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KitchenArticleInstances_KitchenArticles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "KitchenArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KitchenArticleInstances_KitchenStorages_StorageId",
                        column: x => x.StorageId,
                        principalTable: "KitchenStorages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KitchenProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ContainsAlcohol = table.Column<bool>(type: "boolean", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    EventReservationId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KitchenProducts_EventReservations_EventReservationId",
                        column: x => x.EventReservationId,
                        principalTable: "EventReservations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KitchenProducts_KitchenOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "KitchenOrders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SaleItemInstances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    MagazineId = table.Column<Guid>(type: "uuid", nullable: false),
                    Variant = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleItemInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleItemInstances_Purchases_PurchaseId",
                        column: x => x.PurchaseId,
                        principalTable: "Purchases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleItemInstances_SaleItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "SaleItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleItemInstances_ShopMagazines_MagazineId",
                        column: x => x.MagazineId,
                        principalTable: "ShopMagazines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentInstances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReservationPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    EventHallId = table.Column<Guid>(type: "uuid", nullable: true),
                    EventReservationId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentInstances_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentInstances_EventHalls_EventHallId",
                        column: x => x.EventHallId,
                        principalTable: "EventHalls",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipmentInstances_EventReservations_EventReservationId",
                        column: x => x.EventReservationId,
                        principalTable: "EventReservations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LostAndFoundItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    FoundDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    FoundByEmployeeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LostAndFoundItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LostAndFoundItems_EmployeeProfile_FoundByEmployeeId",
                        column: x => x.FoundByEmployeeId,
                        principalTable: "EmployeeProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LostAndFoundItems_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceRequests_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reservation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckInDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CheckOutDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    NumberOfOvernightStays = table.Column<int>(type: "integer", nullable: false),
                    NIP = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SpecialWishes = table.Column<string>(type: "text", nullable: true),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservation_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomCleanings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedEmployeeId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomCleanings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomCleanings_EmployeeProfile_AssignedEmployeeId",
                        column: x => x.AssignedEmployeeId,
                        principalTable: "EmployeeProfile",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoomCleanings_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KitchenRecipes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OutcomeProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenRecipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KitchenRecipes_KitchenProducts_OutcomeProductId",
                        column: x => x.OutcomeProductId,
                        principalTable: "KitchenProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuestReservation",
                columns: table => new
                {
                    GuestsId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReservationsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestReservation", x => new { x.GuestsId, x.ReservationsId });
                    table.ForeignKey(
                        name: "FK_GuestReservation_Guest_GuestsId",
                        column: x => x.GuestsId,
                        principalTable: "Guest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuestReservation_Reservation_ReservationsId",
                        column: x => x.ReservationsId,
                        principalTable: "Reservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "text", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReservationId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoice_Reservation_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Reservation_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Reservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplyUsages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomCleaningId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplyId = table.Column<Guid>(type: "uuid", nullable: false),
                    AmountUsed = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplyUsages_HousekeepingSupplies_SupplyId",
                        column: x => x.SupplyId,
                        principalTable: "HousekeepingSupplies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplyUsages_RoomCleanings_RoomCleaningId",
                        column: x => x.RoomCleaningId,
                        principalTable: "RoomCleanings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KitchenRecipeIngredients",
                columns: table => new
                {
                    RecipeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Count = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenRecipeIngredients", x => new { x.RecipeId, x.ArticleId });
                    table.ForeignKey(
                        name: "FK_KitchenRecipeIngredients_KitchenArticles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "KitchenArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KitchenRecipeIngredients_KitchenRecipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "KitchenRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RaportPayment",
                columns: table => new
                {
                    RaportId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaportPayment", x => new { x.RaportId, x.PaymentId });
                    table.ForeignKey(
                        name: "FK_RaportPayment_Payment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RaportPayment_Raport_RaportId",
                        column: x => x.RaportId,
                        principalTable: "Raport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_HotelId",
                table: "Address",
                column: "HotelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeShifts_EmployeeId",
                table: "EmployeeShifts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_TypeId",
                table: "Equipment",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentInstances_EquipmentId",
                table: "EquipmentInstances",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentInstances_EventHallId",
                table: "EquipmentInstances",
                column: "EventHallId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentInstances_EventReservationId",
                table: "EquipmentInstances",
                column: "EventReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_EventHalls_EventReservationId",
                table: "EventHalls",
                column: "EventReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_EventReservations_EventTypeId",
                table: "EventReservations",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EventReservations_StatusId",
                table: "EventReservations",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_GuestReservation_ReservationsId",
                table: "GuestReservation",
                column: "ReservationsId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_ReservationId",
                table: "Invoice",
                column: "ReservationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KitchenArticleInstances_ArticleId",
                table: "KitchenArticleInstances",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenArticleInstances_StorageId",
                table: "KitchenArticleInstances",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenArticles_TypeId",
                table: "KitchenArticles",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenOrders_TypeId",
                table: "KitchenOrders",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenProducts_EventReservationId",
                table: "KitchenProducts",
                column: "EventReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenProducts_OrderId",
                table: "KitchenProducts",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenRecipeIngredients_ArticleId",
                table: "KitchenRecipeIngredients",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenRecipes_OutcomeProductId",
                table: "KitchenRecipes",
                column: "OutcomeProductId");

            migrationBuilder.CreateIndex(
                name: "IX_LostAndFoundItems_FoundByEmployeeId",
                table: "LostAndFoundItems",
                column: "FoundByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LostAndFoundItems_RoomId",
                table: "LostAndFoundItems",
                column: "RoomId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_RoomId",
                table: "MaintenanceRequests",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ServiceId",
                table: "Payment",
                column: "ServiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Phone_HotelId",
                table: "Phone",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_ShopPointId",
                table: "Purchases",
                column: "ShopPointId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RaportPayment_PaymentId",
                table: "RaportPayment",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_RecreationBookings_FacilityId",
                table: "RecreationBookings",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_RecreationBookings_GuestId",
                table: "RecreationBookings",
                column: "GuestId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_RoomId",
                table: "Reservation",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_EventReservationId",
                table: "Room",
                column: "EventReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_HotelId",
                table: "Room",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomCleanings_AssignedEmployeeId",
                table: "RoomCleanings",
                column: "AssignedEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomCleanings_RoomId",
                table: "RoomCleanings",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItemInstances_ItemId",
                table: "SaleItemInstances",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItemInstances_MagazineId",
                table: "SaleItemInstances",
                column: "MagazineId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItemInstances_PurchaseId",
                table: "SaleItemInstances",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_TypeId",
                table: "SaleItems",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyUsages_RoomCleaningId",
                table: "SupplyUsages",
                column: "RoomCleaningId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyUsages_SupplyId",
                table: "SupplyUsages",
                column: "SupplyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "EmployeeShifts");

            migrationBuilder.DropTable(
                name: "EquipmentInstances");

            migrationBuilder.DropTable(
                name: "GuestReservation");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "KitchenArticleInstances");

            migrationBuilder.DropTable(
                name: "KitchenRecipeIngredients");

            migrationBuilder.DropTable(
                name: "LostAndFoundItems");

            migrationBuilder.DropTable(
                name: "MaintenanceRequests");

            migrationBuilder.DropTable(
                name: "Phone");

            migrationBuilder.DropTable(
                name: "RaportPayment");

            migrationBuilder.DropTable(
                name: "RecreationBookings");

            migrationBuilder.DropTable(
                name: "SaleItemInstances");

            migrationBuilder.DropTable(
                name: "SupplyUsages");

            migrationBuilder.DropTable(
                name: "Equipment");

            migrationBuilder.DropTable(
                name: "EventHalls");

            migrationBuilder.DropTable(
                name: "KitchenStorages");

            migrationBuilder.DropTable(
                name: "KitchenArticles");

            migrationBuilder.DropTable(
                name: "KitchenRecipes");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Raport");

            migrationBuilder.DropTable(
                name: "Guest");

            migrationBuilder.DropTable(
                name: "RecreationFacilities");

            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropTable(
                name: "SaleItems");

            migrationBuilder.DropTable(
                name: "ShopMagazines");

            migrationBuilder.DropTable(
                name: "HousekeepingSupplies");

            migrationBuilder.DropTable(
                name: "RoomCleanings");

            migrationBuilder.DropTable(
                name: "EquipmentTypes");

            migrationBuilder.DropTable(
                name: "KitchenArticleTypes");

            migrationBuilder.DropTable(
                name: "KitchenProducts");

            migrationBuilder.DropTable(
                name: "Reservation");

            migrationBuilder.DropTable(
                name: "ShopPoints");

            migrationBuilder.DropTable(
                name: "SaleItemTypes");

            migrationBuilder.DropTable(
                name: "EmployeeProfile");

            migrationBuilder.DropTable(
                name: "KitchenOrders");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "KitchenOrderTypes");

            migrationBuilder.DropTable(
                name: "EventReservations");

            migrationBuilder.DropTable(
                name: "Hotels");

            migrationBuilder.DropTable(
                name: "EventReservationStatuses");

            migrationBuilder.DropTable(
                name: "EventTypes");
        }
    }
}
