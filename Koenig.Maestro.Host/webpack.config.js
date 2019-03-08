"use strict";
const webpack = require('webpack');
const path = require("path");
const TerserPlugin = require('terser-webpack-plugin');


module.exports = {
    entry: "./Scripts/src/maestro.jsx",
    output: {
        path: path.resolve(__dirname, "./Scripts/js"),
        filename: "maestro.js"
    },
    resolve: {
        extensions: [".ts", ".tsx", ".js", ".jsx", ".json"]
    },
    optimization: {
        minimizer: [
            new TerserPlugin()
        ]
    },
    module: {
        rules: [
            {
                test: /\.(js|jsx)?$/,
                exclude: /node_modules/,
                use: {
                    loader: "babel-loader",
                    query: {
                        presets: ['@babel/preset-react', '@babel/preset-env']
                    }
                }

            },
            { test: /\.tsx?$/, loader: "awesome-typescript-loader" },
            { enforce: "pre", test: /\.js$/, loader: "source-map-loader" },
        ]
    },
    devtool: "inline-source-map",
    plugins: [
        new webpack.ProvidePlugin({
            $: 'jquery',
            jQuery: 'jquery'
        })

    ]
};