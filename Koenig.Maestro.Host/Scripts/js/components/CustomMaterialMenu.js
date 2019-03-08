import * as React from 'react';
import { FontIcon, MenuButton, ListItem, Divider } from 'react-md';
// eslint-disable-next-line react/prop-types
export default ({ row, onDeleteRow }) => {
    const deleteRow = () => {
        onDeleteRow(row);
    };
    return (React.createElement(MenuButton, { id: "menu-button-2", icon: true, simplifiedMenu: false, anchor: {
            x: "inner left",
            y: "overlap",
        }, menuItems: [
            React.createElement(ListItem, { key: 1, primaryText: "Item One" }),
            React.createElement(ListItem, { key: 2, primaryText: "Item Two" }),
            React.createElement(Divider, { key: 3 }),
            React.createElement(ListItem, { key: 4, primaryText: "Delete", leftIcon: React.createElement(FontIcon, { style: { color: 'red' } }, "delete"), onClick: deleteRow }),
        ], centered: true }, "more_vert"));
};
//# sourceMappingURL=CustomMaterialMenu.js.map