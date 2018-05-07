var path = require('path');
module.exports = {
    entry: {
        app: "./temp/scripts/app.js"
    },
    output: {
        path: path.resolve(__dirname, "./wwwroot/assets/scripts"),
        filename: "[name].js"
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                exclude: /(node_modules)/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: ['env']
                    }
                }
            }
        ]
    },
    mode: "development"
};