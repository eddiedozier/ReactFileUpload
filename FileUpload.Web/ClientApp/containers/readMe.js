import React, { Component } from 'react';
import LayoutContentWrapper from '../components/utility/layoutWrapper';
import LayoutContent from '../components/utility/layoutContent';
import { Row, Col } from 'antd';

export default class extends Component {
  render() {
    return (
      <LayoutContentWrapper style={{ height: '100vh' }}>
        <LayoutContent>
          <div>
            <Row>
              <Col span={6} offset={10}><h1>README</h1></Col>
            </Row>
            <Row>
              <Col span={24} offset={4}><p>To run this project locally simply download and run the yarn command in the terminal to restore packages.</p></Col>
            </Row>
          </div>
          
        </LayoutContent>
      </LayoutContentWrapper>
    );
  }
}
