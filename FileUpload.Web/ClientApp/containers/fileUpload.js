import React, { Component } from 'react';
import LayoutContentWrapper from '../components/utility/layoutWrapper';
import LayoutContent from '../components/utility/layoutContent';
import PageHeader from '../components/utility/pageHeader';
import Box from '../components/utility/box';
import LayoutWrapper from '../components/utility/layoutWrapper';
import ContentHolder from '../components/utility/contentHolder';
import { Upload, Icon, message, Button, Row, Col  } from 'antd';

import axios from 'axios';

function beforeUpload(file) {
  // const isJPG = file.type === 'image/jpeg';
  // if (!isJPG) {
  //   message.error('You can only upload JPG file!');
  // }
  const isLt2M = file.size / 1024 / 1024 < 5;
  if (!isLt2M) {
    message.error('Image must smaller than 5MB!');
  }
  return isLt2M;
}

class FileUpload extends React.Component {
  constructor(props){
    super(props);
    this.url = "api/file/";
  }

  state = {
    fileList: [],
    uploading: false
  }

  handleUpload = () => {
    const { fileList } = this.state;
    const formData = new FormData();
    fileList.forEach((file) => {
      formData.append('files', file);
    });

    this.setState({
      uploading: true
    });
  
    const uploadSuccess = (resp) => {
      console.log(resp)
      console.log("fileuploadewd!!!")
      this.setState({uploading: false});
    };
    const uploadFail = (resp) => {
      console.log(resp);
    };
    console.log("TEst")
    axios.post(this.url + "upload", formData)
      .then(uploadSuccess,uploadFail);
  }

  render() {
    const { uploading } = this.state;
    const props = {
      action: this.url + "upload",
      onRemove: (file) => {
        this.setState(({ fileList }) => {
          const index = fileList.indexOf(file);
          const newFileList = fileList.slice();
          newFileList.splice(index, 1);
          return {
            fileList: newFileList,
          };
        });
      },
      beforeUpload: (file) => {
        this.setState(({ fileList }) => ({
          fileList: [...fileList, file],
        }));
        return false;
      },
      fileList: this.state.fileList,
      name: "files"
    };
    return (
      <LayoutWrapper>
        <PageHeader>File Upload</PageHeader>
        <Box>
          <ContentHolder>
            <Row>
              <Col span={12} offset={10}>
                <h2>Upload Your Files</h2>
              </Col>
            </Row>
            <Row>
              <Col span={11} offset={7}>
              <div>
                <Upload {...props} beforeUpload={beforeUpload}>
                  <Button>
                    <Icon type="upload" /> Select File
                  </Button>
                </Upload>
                <Button
                  className="upload-demo-start"
                  type="primary"
                  onClick={this.handleUpload}
                  disabled={this.state.fileList.length === 0}
                  loading={uploading}>
                  {uploading ? 'Uploading' : 'Start Upload' }
                </Button>
              </div>
              </Col>
            </Row>
            
          </ContentHolder>
        </Box>
      </LayoutWrapper>
    );
  }
}

export default FileUpload;