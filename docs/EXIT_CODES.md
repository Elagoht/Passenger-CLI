# Exit Codes

Exit codes are required to specify specific exit situations. This application is design to be implemented to a HTTP server. So decied to make exit codes look like HTTP status codes:

| Status                   | Exit Code | Referring to HTTP |
|--------------------------|-----------|-------------------|
| Success                  | must be 0 | 200               |
| Created                  | must be 0 | 201               |
| See Other                | 33        | 303               |
| Bad Request              | 40        | 400               |
| Unauthorized             | 41        | 401               |
| Forbidden                | 43        | 403               |
| Not Found                | 44        | 404               |
| Not Acceptable           | 46        | 406               |
| Conflict                 | 49        | 409               |
| Unsupported Media Type   | 45        | 415               |
| Internal Server Error    | 50        | 500               |

If you want to know more about HTTP status codes, you can visit [here](https://developer.mozilla.org/en-US/docs/Web/HTTP/Status).

## Ask for Help

Help wanted! If you can map every verb command to their exit codes, edit this file and please create a pull request. Thank you!
